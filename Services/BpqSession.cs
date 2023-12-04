using packetmail_api.Models;
using PrimS.Telnet;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace packetmail_api.Services;

public class BpqSession() : IDisposable
{
    public string? BbsCall { get; private set; }
    public string? BbsAlias { get; private set; }
    public BpqSessionState State { get; private set; }

    private Client? _client;
    private string? BbsPrompt => BbsCall == null ? null : $"de {BbsCall}>";

    private readonly SemaphoreSlim _semaphoreSlim = new(1);


    public async Task<bool> LoginToNode(string host, int port, string username, string password, string nodePrompt)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (State != BpqSessionState.NotLoggedIn) throw new InvalidOperationException("Already logged in");
            State = BpqSessionState.LoginFailed;

            _client = new Client(host, port, CancellationToken.None);
            var fromServer = await _client.TerminatedReadAsync(":", TimeSpan.FromSeconds(5));
            if (fromServer != "user:")
            {
                throw new BpqProtocolException($"Expected \"user:\", got \"{fromServer}\"");
            }
            await _client.WriteLineRfc854Async(username);
            fromServer = (await _client.TerminatedReadAsync(":", TimeSpan.FromSeconds(5)))?.Trim();
            if (fromServer == "user:")
            {
                return false;
            }
            else if (fromServer != "password:")
            {
                throw new BpqProtocolException($"Expected \"password:\", got \"{fromServer}\"");
            }
            await _client.WriteLineRfc854Async(password);

            var expect = "\r\n" + nodePrompt.Replace("\\n", "\r\n") + "\r";
            var actual = await _client.TerminatedReadAsync(expect, TimeSpan.FromSeconds(5));

            if (actual.Trim() == "password:")
            {
                return false;
            }
            else if (expect != actual)
            {
                throw new BpqProtocolException($"Expected node prompt \"{expect}\", received \"{actual}\"");
            }

            State = BpqSessionState.LoggedInToNode;
            return true;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<bool> EnterBbs()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (State == BpqSessionState.EnteredBbs) return true;
            if (State != BpqSessionState.LoggedInToNode) throw new InvalidOperationException("Not logged in to node");
            State = BpqSessionState.Faulted;
            var client = _client!;
            await client.WriteLineRfc854Async("bbs");

            var lines = new List<string>();
            var cts = new CancellationTokenSource(10000);
            while (!cts.IsCancellationRequested)
            {
                var read = await client.TerminatedReadAsync("\r\n", TimeSpan.FromSeconds(5));
                var parts = read.Split("\r\n").Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim());
                lines.AddRange(parts);

                if (lines.Count >= 2)
                {
                    if (lines[0].Contains("Connected to BBS"))
                    {
                        var line1parts = lines[0].Split(':', '}');
                        BbsAlias = line1parts[0];
                        BbsCall = line1parts[1];
                        if (string.IsNullOrWhiteSpace(BbsCall))
                        {
                            throw new BpqProtocolException($"Could not determine BBS call from \"{lines[0]}\"");
                        }

                        if (lines.Last().EndsWith(BbsPrompt!))
                        {
                            break;
                        }
                    }
                }
            }

            if (cts.IsCancellationRequested)
            {
                throw new BpqProtocolException($"Failed to enter BBS in time; received: {lines.Last()}");
            }

            State = BpqSessionState.EnteredBbs;
            return true;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<List<MailSummary>> GetMyMailSummary()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (State != BpqSessionState.EnteredBbs) throw new InvalidOperationException($"Not in BBS. Call {nameof(EnterBbs)}() first.");
            var client = _client!;

            string[] lines;
            while (true)
            {
                await client.WriteLineRfc854Async("lm");
                var text = await client.TerminatedReadAsync(BbsPrompt! + "\r\n", TimeSpan.FromSeconds(5));
                lines = text.Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (lines[0] == "Invalid Command") // BPQ randomly does this when sent "lm"
                {
                    continue;
                }

                break;
            }

            var result = new List<MailSummary>();
            foreach (var line in lines.Take(lines.Length - 1))
            {
                bool parsed;
                MailSummary summary;
                try
                {
                    parsed = MailSummary.TryParse(line, out summary);
                }
                catch (Exception ex)
                {
                    State = BpqSessionState.Faulted;
                    throw new BpqProtocolException($"Could not parse as \"lm\" result: {line}; {ex.Message}");
                }

                if (parsed)
                {
                    result.Add(summary);
                }
                else
                {
                    State = BpqSessionState.Faulted;
                    throw new BpqProtocolException($"Could not parse as \"lm\" result: {line}");
                }
            }

            return result;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    internal async Task<bool> TestState()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            if (disposed || _client == null) return false;
            if (State == BpqSessionState.Faulted || State == BpqSessionState.LoginFailed) return false;

            if (State == BpqSessionState.LoggedInToNode)
            {
                await _client.WriteLineRfc854Async("?");
                var read = await _client.TerminatedReadAsync("\r\n", TimeSpan.FromSeconds(5));
                if (string.IsNullOrWhiteSpace(read))
                {
                    return false;
                }

                return true;
            }
            else if (State == BpqSessionState.EnteredBbs)
            {
                await _client.WriteLineRfc854Async("");
                var read = await _client.TerminatedReadAsync(BbsPrompt!, TimeSpan.FromSeconds(5));
                if (string.IsNullOrWhiteSpace(read))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        if (_client == null) return;
        ((IDisposable)_client).Dispose();
    }

    private bool disposed;

    public enum BpqSessionState
    {
        NotLoggedIn,
        LoginFailed,
        LoggedInToNode,
        EnteredBbs,
        Faulted,
    }
}
