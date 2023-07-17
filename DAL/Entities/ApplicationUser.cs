using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public RefreshToken? RefreshToken { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
