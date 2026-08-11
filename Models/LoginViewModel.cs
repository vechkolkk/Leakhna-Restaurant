using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter the email address for your account.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address, like name@example.com.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
