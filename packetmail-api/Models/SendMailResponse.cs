namespace packetmail_api.Models;

public class SendMailResponse(string bid, int id, int size)
{
    public int Id { get; } = id;
    public string Bid { get; } = bid;
    public int Size { get; } = size;
}