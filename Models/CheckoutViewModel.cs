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

    [Display(Name = "Cardholder name")]
    public string? CardholderName { get; set; }

    [Display(Name = "Card number")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiry")]
    public string? CardExpiry { get; set; }

    [Display(Name = "CVV")]
    public string? CardCvv { get; set; }

    [EmailAddress]
    [Display(Name = "PayPal email")]
    public string? PayPalEmail { get; set; }

    [Display(Name = "E-transfer sender name")]
    public string? ETransferSenderName { get; set; }

    [Display(Name = "E-transfer reference")]
    public string? ETransferReference { get; set; }

    [Required]
    [Display(Name = "Order type")]
    public string OrderType { get; set; } = "Pickup";

    [Display(Name = "Delivery address")]
    public string? DeliveryAddress { get; set; }

    [Display(Name = "Checkout as guest")]
    public bool IsGuestCheckout { get; set; } = true;

    [Display(Name = "Order notes")]
    public string? Notes { get; set; }
}
