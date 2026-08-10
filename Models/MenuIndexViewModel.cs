namespace RestaurantApp.Models;

public class MenuIndexViewModel
{
    public IReadOnlyList<MenuItem> Items { get; set; } = [];

    public IReadOnlyList<string> Categories { get; set; } = [];

    public string? Category { get; set; }

    public string? Search { get; set; }

    public bool AvailableOnly { get; set; } = true;
}
