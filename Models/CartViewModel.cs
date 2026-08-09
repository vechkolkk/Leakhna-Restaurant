namespace RestaurantApp.Models;

public class CartViewModel
{
    public IReadOnlyList<CartLine> Lines { get; set; } = [];

    public decimal Total => Lines.Sum(line => line.LineTotal);

    public int ItemCount => Lines.Sum(line => line.Quantity);
}
