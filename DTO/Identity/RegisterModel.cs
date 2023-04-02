using DTO.Validation;
using System.ComponentModel.DataAnnotations;

namespace DTO.Identity;

public class RegisterModel
{
    [Required(ErrorMessage = "This field is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\w\\W]{8,}$", ErrorMessage = "Please enter a valid password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\w\\W]{8,}$", ErrorMessage = "Please enter a valid password")]
    [Compare(nameof(Password), ErrorMessage = "Password and ConfirmPassword do not match")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [ValidValues("User", "Admin")]
    public string Role { get; set; }

    public RegisterModel()
    {
        FirstName = String.Empty;
        LastName = String.Empty;
        Email = String.Empty;
        Password = String.Empty;
        Role = String.Empty;
        ConfirmPassword = String.Empty;
    }
}
