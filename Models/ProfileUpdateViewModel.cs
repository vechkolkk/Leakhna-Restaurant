using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class ProfileUpdateViewModel
{
    [Required(ErrorMessage = "Enter your full name.")]
    [StringLength(80, ErrorMessage = "Full name must be 80 characters or fewer.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [StringLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
    public string? Phone { get; set; }

    [StringLength(180, ErrorMessage = "Default address must be 180 characters or fewer.")]
    [Display(Name = "Default address")]
    public string? DefaultAddress { get; set; }
}
