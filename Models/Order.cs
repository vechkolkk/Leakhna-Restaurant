namespace RestaurantApp.Models;

public class Order
{
    public string Id { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string CustomerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public bool IsGuestCheckout { get; set; }

    public string Status { get; set; } = "Paid";

    public string? Notes { get; set; }

    public IReadOnlyList<CartLine> Lines { get; set; } = [];

    public decimal Total => Lines.Sum(line => line.LineTotal);
}
