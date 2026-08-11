namespace RestaurantApp.Models;

public class MenuItem
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IReadOnlyList<string> Ingredients { get; set; } = [];

    public IReadOnlyList<string> DietaryTags { get; set; } = [];

    public IReadOnlyList<string> Allergens { get; set; } = [];

    public int EstimatedCalories { get; set; }

    public int EstimatedPrepMinutes { get; set; } = 15;

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    public string AvailabilityLevel { get; set; } = MenuAvailability.Regular;

    public string AccentClass { get; set; } = "accent-red";

    public bool IsOrderable => AvailabilityLevel is MenuAvailability.Regular or MenuAvailability.Limited &&
        IsAvailable;

    public bool IsLimited => IsAvailable && AvailabilityLevel == MenuAvailability.Limited;

    public string AvailabilityStatus
    {
        get
        {
            if (!IsAvailable)
            {
                return MenuAvailability.Unavailable;
            }

            if (string.IsNullOrWhiteSpace(AvailabilityLevel))
            {
                return MenuAvailability.Regular;
            }

            return AvailabilityLevel;
        }
    }
}
