using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class MenuItemFormViewModel
{
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Ingredients")]
    public string IngredientsText { get; set; } = string.Empty;

    [Range(0, 5000)]
    [Display(Name = "Estimated calories")]
    public int EstimatedCalories { get; set; }

    [Range(0.01, 1000)]
    public decimal Price { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    [Required]
    [Display(Name = "Availability level")]
    public string AvailabilityLevel { get; set; } = MenuAvailability.Regular;

    [Display(Name = "Color accent")]
    public string AccentClass { get; set; } = "accent-red";

    public static MenuItemFormViewModel FromMenuItem(MenuItem item)
    {
        return new MenuItemFormViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Description = item.Description,
            IngredientsText = string.Join(", ", item.Ingredients),
            EstimatedCalories = item.EstimatedCalories,
            Price = item.Price,
            IsAvailable = item.IsAvailable,
            AvailabilityLevel = item.AvailabilityStatus,
            AccentClass = item.AccentClass
        };
    }

    public MenuItem ToMenuItem(string? id = null)
    {
        return new MenuItem
        {
            Id = string.IsNullOrWhiteSpace(id) ? CreateSlug(Name) : id,
            Name = Name.Trim(),
            Category = Category.Trim(),
            Description = Description.Trim(),
            Ingredients = IngredientsText
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList(),
            EstimatedCalories = EstimatedCalories,
            Price = Price,
            IsAvailable = IsAvailable,
            AvailabilityLevel = AvailabilityLevel,
            AccentClass = AccentClass
        };
    }

    private static string CreateSlug(string value)
    {
        var slug = new string(value
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray());

        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return slug.Trim('-');
    }
}
