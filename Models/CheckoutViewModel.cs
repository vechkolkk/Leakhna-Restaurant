using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Enter the name for this order.")]
    [StringLength(80, ErrorMessage = "Customer name must be 80 characters or fewer.")]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address for the receipt.")]
    [StringLength(120, ErrorMessage = "Email must be 120 characters or fewer.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [StringLength(30, ErrorMessage = "Phone number must be 30 characters or fewer.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Choose a payment method.")]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "Credit Card";

    [StringLength(80, ErrorMessage = "Cardholder name must be 80 characters or fewer.")]
    [Display(Name = "Cardholder name")]
    public string? CardholderName { get; set; }

    [RegularExpression(@"^[0-9 -]{12,23}$", ErrorMessage = "Enter a card number using digits, spaces, or dashes.")]
    [Display(Name = "Card number")]
    public string? CardNumber { get; set; }

    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Enter expiry as MM/YY.")]
    [Display(Name = "Expiry")]
    public string? CardExpiry { get; set; }

    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Enter a 3 or 4 digit CVV.")]
    [Display(Name = "CVV")]
    public string? CardCvv { get; set; }

    [EmailAddress(ErrorMessage = "Enter a valid PayPal email address.")]
    [StringLength(120, ErrorMessage = "PayPal email must be 120 characters or fewer.")]
    [Display(Name = "PayPal email")]
    public string? PayPalEmail { get; set; }

    [StringLength(80, ErrorMessage = "Sender name must be 80 characters or fewer.")]
    [Display(Name = "E-transfer sender name")]
    public string? ETransferSenderName { get; set; }

    [StringLength(60, ErrorMessage = "Reference must be 60 characters or fewer.")]
    [Display(Name = "E-transfer reference")]
    public string? ETransferReference { get; set; }

    [Required(ErrorMessage = "Choose pickup or delivery.")]
    [Display(Name = "Order type")]
    public string OrderType { get; set; } = "Pickup";

    [StringLength(180, ErrorMessage = "Delivery address must be 180 characters or fewer.")]
    [Display(Name = "Delivery address")]
    public string? DeliveryAddress { get; set; }

    [Required(ErrorMessage = "Choose when you want the order.")]
    [Display(Name = "Fulfillment time")]
    public string FulfillmentTiming { get; set; } = "ASAP";

    [Display(Name = "Requested time")]
    public DateTime? RequestedFulfillmentAt { get; set; }

    [Display(Name = "Checkout as guest")]
    public bool IsGuestCheckout { get; set; } = true;

    [StringLength(300, ErrorMessage = "Order notes must be 300 characters or fewer.")]
    [Display(Name = "Order notes")]
    public string? Notes { get; set; }
}
