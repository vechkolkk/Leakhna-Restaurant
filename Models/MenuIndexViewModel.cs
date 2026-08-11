namespace RestaurantApp.Models;

public class MenuIndexViewModel
{
    public IReadOnlyList<MenuItem> Items { get; set; } = [];

    public IReadOnlyDictionary<string, ReviewSummary> ReviewSummaries { get; set; } =
        new Dictionary<string, ReviewSummary>();

    public IReadOnlySet<string> FavoriteMenuItemIds { get; set; } = new HashSet<string>();

    public IReadOnlyList<string> Categories { get; set; } = [];

    public IReadOnlyList<string> DietaryTags { get; set; } = [];

    public IReadOnlyList<string> Allergens { get; set; } = [];

    public string? Category { get; set; }

    public string? DietaryTag { get; set; }

    public string? AvoidAllergen { get; set; }

    public string? Search { get; set; }

    public bool AvailableOnly { get; set; } = true;

    public bool FavoritesOnly { get; set; }
}
