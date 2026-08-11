namespace RestaurantApp.Models;

public class MenuIndexViewModel
{
    public IReadOnlyList<MenuItem> Items { get; set; } = [];

    public IReadOnlyList<string> Categories { get; set; } = [];

    public IReadOnlyList<string> DietaryTags { get; set; } = [];

    public IReadOnlyList<string> Allergens { get; set; } = [];

    public string? Category { get; set; }

    public string? DietaryTag { get; set; }

    public string? AvoidAllergen { get; set; }

    public string? Search { get; set; }

    public bool AvailableOnly { get; set; } = true;
}
