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

    public string AccentClass { get; set; } = "accent-red";
}
