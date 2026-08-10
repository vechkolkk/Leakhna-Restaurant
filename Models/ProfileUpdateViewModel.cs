using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class ProfileUpdateViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Display(Name = "Default address")]
    public string? DefaultAddress { get; set; }
}
