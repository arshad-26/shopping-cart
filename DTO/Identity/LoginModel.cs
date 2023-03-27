using System.ComponentModel.DataAnnotations;

namespace DTO.Identity;

public class LoginModel
{
    [Required(ErrorMessage = "This field is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public string Password { get; set; }

    public LoginModel()
    {
        Email= String.Empty;
        Password= String.Empty;
    }
}
