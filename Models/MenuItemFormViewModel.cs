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

    [Range(0, 10000)]
    [Display(Name = "Stock quantity")]
    public int StockQuantity { get; set; } = 20;

    [Range(0, 10000)]
    [Display(Name = "Low-stock alert")]
    public int LowStockThreshold { get; set; } = 5;

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
            StockQuantity = item.StockQuantity,
            LowStockThreshold = item.LowStockThreshold,
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
            StockQuantity = StockQuantity,
            LowStockThreshold = LowStockThreshold,
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
