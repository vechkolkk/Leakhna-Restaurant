using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryMenuService : IMenuService
{
    private readonly List<MenuItem> _menuItems = SeedData.MenuItems
        .Select(SeedData.CloneMenuItem)
        .ToList();

    public IReadOnlyList<MenuItem> GetMenuItems()
    {
        return _menuItems.ToList();
    }

    public MenuItem? GetMenuItem(string id)
    {
        return _menuItems.FirstOrDefault(item => item.Id == id);
    }

    public IReadOnlyList<string> GetCategories()
    {
        return _menuItems.Select(item => item.Category).Distinct().OrderBy(category => category).ToList();
    }

    public IReadOnlyList<string> GetDietaryTags()
    {
        return _menuItems
            .SelectMany(item => item.DietaryTags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag)
            .ToList();
    }

    public IReadOnlyList<string> GetAllergens()
    {
        return _menuItems
            .SelectMany(item => item.Allergens)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(allergen => allergen)
            .ToList();
    }

    public MenuItem AddMenuItem(MenuItem item)
    {
        item.Id = EnsureUniqueId(item.Id);
        _menuItems.Add(item);
        return item;
    }

    public bool UpdateMenuItem(MenuItem item)
    {
        var index = _menuItems.FindIndex(existing => existing.Id == item.Id);

        if (index < 0)
        {
            return false;
        }

        _menuItems[index] = item;
        return true;
    }

    public bool DeleteMenuItem(string id)
    {
        var item = GetMenuItem(id);

        if (item is null)
        {
            return false;
        }

        _menuItems.Remove(item);
        return true;
    }

    private string EnsureUniqueId(string id)
    {
        var candidate = id;
        var suffix = 2;

        while (_menuItems.Any(item => item.Id == candidate))
        {
            candidate = $"{id}-{suffix}";
            suffix++;
        }

        return candidate;
    }

    public bool ValidateAvailability(IReadOnlyList<CartLine> lines, out string? message)
    {
        foreach (var line in lines)
        {
            var item = GetMenuItem(line.MenuItem.Id);

            if (item is null || !item.IsOrderable)
            {
                message = $"{line.MenuItem.Name} is currently {line.MenuItem.AvailabilityStatus.ToLowerInvariant()}.";
                return false;
            }
        }

        message = null;
        return true;
    }
}
