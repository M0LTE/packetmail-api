namespace packetmail_api.Models;

public class SendMailRequest
{
    public string To { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
}
