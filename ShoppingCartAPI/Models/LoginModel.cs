using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models;

public class LoginModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public LoginModel()
    {
        Email= String.Empty;
        Password= String.Empty;
    }
}
