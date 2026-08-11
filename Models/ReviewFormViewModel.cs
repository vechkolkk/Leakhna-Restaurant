using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class ReviewFormViewModel
{
    [Required(ErrorMessage = "Enter the name to show with your review.")]
    [StringLength(80, ErrorMessage = "Reviewer name must be 80 characters or fewer.")]
    [Display(Name = "Your name")]
    public string ReviewerName { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Choose a rating from 1 to 5.")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Write a short review.")]
    [StringLength(500, ErrorMessage = "Review must be 500 characters or fewer.")]
    public string Comment { get; set; } = string.Empty;
}
