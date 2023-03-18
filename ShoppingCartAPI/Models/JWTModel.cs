namespace ShoppingCartAPI.Models;

public class JWTModel
{
    public string ValidIssuer { get; set; }

    public string ValidAudience { get; set; }

    public string Secret { get; set; }

    public JWTModel()
    {
		ValidIssuer = String.Empty;
		ValidAudience = String.Empty;
        Secret = String.Empty;
	}
}
