namespace RestaurantApp.Models;

public class CartViewModel
{
    public const decimal HstRate = 0.13m;

    public IReadOnlyList<CartLine> Lines { get; set; } = [];

    public decimal Subtotal => Lines.Sum(line => line.LineTotal);

    public decimal TaxRate => HstRate;

    public decimal Tax => Math.Round(Subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);

    public decimal Total => Subtotal + Tax;

    public int ItemCount => Lines.Sum(line => line.Quantity);
}
