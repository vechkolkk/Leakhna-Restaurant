namespace RestaurantApp.Models;

public class MenuItem
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IReadOnlyList<string> Ingredients { get; set; } = [];

    public int EstimatedCalories { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int StockQuantity { get; set; } = 20;

    public int LowStockThreshold { get; set; } = 5;

    public string AccentClass { get; set; } = "accent-red";

    public bool IsOrderable => IsAvailable && StockQuantity > 0;

    public bool IsLowStock => IsOrderable && StockQuantity <= LowStockThreshold;

    public string InventoryStatus
    {
        get
        {
            if (!IsAvailable)
            {
                return "Unavailable";
            }

            if (StockQuantity <= 0)
            {
                return "Sold out";
            }

            return IsLowStock ? $"Only {StockQuantity} left" : $"{StockQuantity} in stock";
        }
    }
}
