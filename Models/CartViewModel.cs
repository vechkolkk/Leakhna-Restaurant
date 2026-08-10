namespace RestaurantApp.Models;

public class CartViewModel
{
    public const decimal HstRate = 0.13m;

    public IReadOnlyList<CartLine> Lines { get; set; } = [];

    public Promotion? Promotion { get; set; }

    public string? PromoCode => Promotion?.Code;

    public decimal Subtotal => Lines.Sum(line => line.LineTotal);

    public decimal Discount => Promotion?.CalculateDiscount(Subtotal) ?? 0;

    public decimal DiscountedSubtotal => Math.Max(0, Subtotal - Discount);

    public decimal TaxRate => HstRate;

    public decimal Tax => Math.Round(DiscountedSubtotal * TaxRate, 2, MidpointRounding.AwayFromZero);

    public decimal Total => DiscountedSubtotal + Tax;

    public int ItemCount => Lines.Sum(line => line.Quantity);
}
