using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class ReviewFormViewModel
{
    [Required]
    [Display(Name = "Your name")]
    public string ReviewerName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required]
    [StringLength(500)]
    public string Comment { get; set; } = string.Empty;
}
