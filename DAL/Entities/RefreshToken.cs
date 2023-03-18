namespace DAL.Entities;

public class RefreshToken
{
    public string UserId { get; set; }

	public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }

    public ApplicationUser User { get; set; }

    public RefreshToken()
    {
        UserId = String.Empty;
        Token = String.Empty;
        User= new ApplicationUser();
    }
}
