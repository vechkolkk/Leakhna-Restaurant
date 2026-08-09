namespace RestaurantApp.Models;

public class CartLine
{
    public required MenuItem MenuItem { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => MenuItem.Price * Quantity;
}
