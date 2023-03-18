namespace ShoppingCartAPI.Models;

public class TokenModel
{
    public string Token { get; set; }

    public string RefreshToken { get; set;}

    public TokenModel()
    {
        Token = String.Empty;
        RefreshToken = String.Empty;
    }
}
