using Microsoft.Extensions.Options;
using packetmail_api.Configuration;
using System.Collections.Concurrent;

namespace packetmail_api.Services;

public class BpqSessionManager(IOptions<ServiceConfig> serviceConfig)
{
    private readonly ConcurrentDictionary<string, BpqSession> sessionStore = new();

    public async Task<BpqSession?> RetrieveSessionAsync(string sessionId)
    {
        if (sessionStore.TryGetValue(sessionId, out var session))
        {
            if (!await session.TestState())
            {
                return null;
            }

            return session;
        }

        return null;
    }

    public async Task<string?> CreateSession(string username, string password)
    {
        var session = new BpqSession();
        var validCredentials = await session.LoginToNode(serviceConfig.Value.BpqHost!, serviceConfig.Value.BpqTelnetPort, username, password, serviceConfig.Value.NodeWelcomeText!);
        if (validCredentials)
        {
            var token = Guid.NewGuid().ToString();
            sessionStore.TryAdd(token, session);
            return token;
        }
        return null;
    }
}
