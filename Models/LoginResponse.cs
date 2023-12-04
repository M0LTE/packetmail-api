namespace packetmail_api.Models;

public class LoginResponse(string? sessionToken)
{
    public static LoginResponse Failure => new(null) { Success = false };

    public string? SessionToken { get; } = sessionToken;
    public bool Success { get; private set; } = true;
}