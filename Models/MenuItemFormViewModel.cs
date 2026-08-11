using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class MenuItemFormViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Enter a dish name.")]
    [StringLength(80, ErrorMessage = "Dish name must be 80 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a category.")]
    [StringLength(40, ErrorMessage = "Category must be 40 characters or fewer.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a short dish description.")]
    [StringLength(300, ErrorMessage = "Description must be 300 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ingredients must be 500 characters or fewer.")]
    [Display(Name = "Ingredients")]
    public string IngredientsText { get; set; } = string.Empty;

    [StringLength(240, ErrorMessage = "Dietary tags must be 240 characters or fewer.")]
    [Display(Name = "Dietary tags")]
    public string DietaryTagsText { get; set; } = string.Empty;

    [StringLength(240, ErrorMessage = "Allergens must be 240 characters or fewer.")]
    [Display(Name = "Allergens")]
    public string AllergensText { get; set; } = string.Empty;

    [Range(0, 5000, ErrorMessage = "Calories must be between 0 and 5000.")]
    [Display(Name = "Estimated calories")]
    public int EstimatedCalories { get; set; }

    [Range(1, 180, ErrorMessage = "Prep time must be between 1 and 180 minutes.")]
    [Display(Name = "Prep minutes")]
    public int EstimatedPrepMinutes { get; set; } = 15;

    [Range(0.01, 1000, ErrorMessage = "Price must be between $0.01 and $1000.")]
    public decimal Price { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    [Required(ErrorMessage = "Choose an availability level.")]
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
            DietaryTagsText = string.Join(", ", item.DietaryTags),
            AllergensText = string.Join(", ", item.Allergens),
            EstimatedCalories = item.EstimatedCalories,
            EstimatedPrepMinutes = item.EstimatedPrepMinutes,
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
            Ingredients = SplitCsv(IngredientsText),
            DietaryTags = SplitCsv(DietaryTagsText),
            Allergens = SplitCsv(AllergensText),
            EstimatedCalories = EstimatedCalories,
            EstimatedPrepMinutes = EstimatedPrepMinutes,
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

    private static IReadOnlyList<string> SplitCsv(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
