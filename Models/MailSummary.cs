namespace packetmail_api.Models;

public class MailSummary
{
    public int Id { get; private set; }
    public string? Date { get; private set; }
    public string? Type { get; private set; }
    public int Length { get; private set; }
    public string? DestinationCall { get; private set; }
    public string? DestinationBbs { get; private set; }
    public string? SourceCall { get; private set; }
    public string? Subject { get; private set; }

    public static bool TryParse(string line, out MailSummary summary)
    {
        var idField = line[..6].Trim();
        var dateField = line.Substring(7, 6);
        var typeField = line.Substring(14, 2);
        var sizeField = line.Substring(17, 7);
        var destCallField = line.Substring(25, 6);
        var destBbsField = line.Substring(32, 7);
        var sourceCallField = line.Substring(40, 6);
        var subjectField = line[47..].Trim();

        summary = new MailSummary();

        if (!int.TryParse(idField, out var id))
        {
            return false;
        }

        if (!int.TryParse(sizeField, out var size))
        {
            return false;
        }

        summary.Id = id;
        summary.Date = dateField;
        summary.Type = typeField;
        summary.Length = size;
        summary.DestinationCall = destCallField.Trim();
        summary.DestinationBbs = destBbsField.Trim();
        summary.SourceCall = sourceCallField.Trim();
        summary.Subject = subjectField;

        return true;
    }
}