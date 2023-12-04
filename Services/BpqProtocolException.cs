using System.Runtime.Serialization;

namespace packetmail_api.Services;

public class BpqProtocolException : Exception
{
    public BpqProtocolException()
    {
    }

    public BpqProtocolException(string? message) : base(message)
    {
    }

    public BpqProtocolException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BpqProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}