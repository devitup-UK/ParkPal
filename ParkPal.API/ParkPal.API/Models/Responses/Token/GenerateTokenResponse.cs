namespace ParkPal.API.Models.Responses.Token;

public class GenerateTokenResponse
{
    public string Token { get; set; }

    public GenerateTokenResponse(string token)
    {
        Token = token;
    }
}