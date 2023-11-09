namespace WebApi.DTOs;
public class TokenRequest
{
    public TokenRequest(string token)
    {
        Token = token;
    }
    public string Token { get; set; } = default!;
}
