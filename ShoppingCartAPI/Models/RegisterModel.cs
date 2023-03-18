using ShoppingCartAPI.Validation;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models;

public class RegisterModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
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
