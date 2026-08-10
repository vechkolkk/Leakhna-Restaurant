namespace RestaurantApp.Models;

public class TopSellingDishItem
{
    public string MenuItemId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int QuantitySold { get; set; }

    public decimal Revenue { get; set; }
}
