namespace packetmail_api.Configuration;

public class ServiceConfig
{
    public string? NODE_TELNET_CTEXT { get; set; }
    public string? BPQ_HOST { get; set; }
    public int BPQ_TELNET_PORT { get; set; }

    public string? NodeWelcomeText => NODE_TELNET_CTEXT;
    public string? BpqHost => BPQ_HOST;
    public int BpqTelnetPort => BPQ_TELNET_PORT;
}
