namespace RestaurantApp.Models;

public class Order
{
    public string Id { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string CustomerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? CustomerId { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = "Paid";

    public string? PaymentSummary { get; set; }

    public string OrderType { get; set; } = "Pickup";

    public string? DeliveryAddress { get; set; }

    public string FulfillmentTiming { get; set; } = "ASAP";

    public DateTime? RequestedFulfillmentAt { get; set; }

    public bool IsGuestCheckout { get; set; }

    public string Status { get; set; } = "Paid";

    public string? Notes { get; set; }

    public IReadOnlyList<CartLine> Lines { get; set; } = [];

    public decimal Subtotal { get; set; }

    public decimal TaxRate { get; set; } = CartViewModel.HstRate;

    public decimal Tax { get; set; }

    public decimal Total { get; set; }
}
