using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class CheckoutViewModel
{
    [Required]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [Required]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "Credit Card";

    [Display(Name = "Checkout as guest")]
    public bool IsGuestCheckout { get; set; } = true;

    [Display(Name = "Order notes")]
    public string? Notes { get; set; }
}
