using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter your full name.")]
    [StringLength(80, ErrorMessage = "Full name must be 80 characters or fewer.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter an email address.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address, like name@example.com.")]
    [StringLength(120, ErrorMessage = "Email must be 120 characters or fewer.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [StringLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
    public string? Phone { get; set; }

    [StringLength(180, ErrorMessage = "Default address must be 180 characters or fewer.")]
    [Display(Name = "Default address")]
    public string? DefaultAddress { get; set; }

    [Required(ErrorMessage = "Create a password.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords must match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
