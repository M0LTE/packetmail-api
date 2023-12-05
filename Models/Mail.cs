
using System.Text;

namespace packetmail_api.Models;

public class Mail
{
    public string? From { get; private set; }
    public string? To { get; private set; }
    public string? TypeStatus { get; private set; }
    /// <summary>
    /// Takes the format 16-Nov 12:26Z
    /// </summary>
    public string? DateTime { get; private set; }
    public string? Bid { get; private set; }
    public string? Title { get; private set; }
    public List<string>? Routing { get; private set; }
    public string? Body { get; private set; }

    public static bool TryParse(int id, string[] lines, out Mail mail)
    {
        Dictionary<string, string> headers = [];
        List<string> routing = [];
        StringBuilder bodyBuilder = new();
        mail = new();

        var blankLines = 0;
        foreach (var line in lines)
        {
            if (line.StartsWith($"[End of Message #{id} from "))
            {
                break;
            }

            if (blankLines == 0)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    blankLines++;
                    continue;
                }
                else
                {
                    var colonIndex = line.IndexOf(':');
                    var key = line[..colonIndex];
                    var value = line[(colonIndex + 2)..];
                    if (headers.ContainsKey(key))
                    {
                        throw new Exception($"Message already contains header {key}");
                    }
                    headers[key] = value;
                }
            }

            if (blankLines == 1)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    blankLines++;
                    continue;
                }
                else
                {
                    routing.Add(line);
                }
            }

            if (blankLines == 2)
            {
                bodyBuilder.Append(line + "\n");
            }
        }

        mail.From = GetValue(headers, "From");
        mail.To = GetValue(headers, "To");
        mail.TypeStatus = GetValue(headers, "Type/Status");
        mail.DateTime = GetValue(headers, "Date/Time");
        mail.Bid = GetValue(headers, "Bid");
        mail.Title = GetValue(headers, "Title");

        mail.Routing = routing;

        mail.Body = bodyBuilder.ToString().Trim();

        return true;
    }

    private static string? GetValue(Dictionary<string, string> headers, string key)
    {
        if (headers.TryGetValue(key, out string? value))
        {
            return value;
        }

        return null;
    }
}
