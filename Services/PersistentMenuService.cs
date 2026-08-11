using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class PersistentMenuService : IMenuService
{
    private readonly IRestaurantDataStore _dataStore;

    public PersistentMenuService(IRestaurantDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<MenuItem> GetMenuItems()
    {
        return _dataStore.GetSnapshot().MenuItems;
    }

    public MenuItem? GetMenuItem(string id)
    {
        return GetMenuItems().FirstOrDefault(item => item.Id == id);
    }

    public IReadOnlyList<string> GetCategories()
    {
        return GetMenuItems().Select(item => item.Category).Distinct().OrderBy(category => category).ToList();
    }

    public IReadOnlyList<string> GetDietaryTags()
    {
        return GetMenuItems()
            .SelectMany(item => item.DietaryTags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag)
            .ToList();
    }

    public IReadOnlyList<string> GetAllergens()
    {
        return GetMenuItems()
            .SelectMany(item => item.Allergens)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(allergen => allergen)
            .ToList();
    }

    public MenuItem AddMenuItem(MenuItem item)
    {
        var snapshot = _dataStore.GetSnapshot();
        item.Id = EnsureUniqueId(item.Id, snapshot.MenuItems);
        snapshot.MenuItems.Add(item);
        _dataStore.SaveSnapshot(snapshot);
        return item;
    }

    public bool UpdateMenuItem(MenuItem item)
    {
        var snapshot = _dataStore.GetSnapshot();
        var index = snapshot.MenuItems.FindIndex(existing => existing.Id == item.Id);

        if (index < 0)
        {
            return false;
        }

        snapshot.MenuItems[index] = item;
        _dataStore.SaveSnapshot(snapshot);
        return true;
    }

    public bool DeleteMenuItem(string id)
    {
        var snapshot = _dataStore.GetSnapshot();
        var item = snapshot.MenuItems.FirstOrDefault(existing => existing.Id == id);

        if (item is null)
        {
            return false;
        }

        snapshot.MenuItems.Remove(item);
        _dataStore.SaveSnapshot(snapshot);
        return true;
    }

    public bool ValidateAvailability(IReadOnlyList<CartLine> lines, out string? message)
    {
        var snapshot = _dataStore.GetSnapshot();

        foreach (var line in lines)
        {
            var item = snapshot.MenuItems.FirstOrDefault(existing => existing.Id == line.MenuItem.Id);

            if (item is null || !item.IsOrderable)
            {
                message = $"{line.MenuItem.Name} is currently {line.MenuItem.AvailabilityStatus.ToLowerInvariant()}.";
                return false;
            }
        }

        message = null;
        return true;
    }

    private static string EnsureUniqueId(string id, IReadOnlyList<MenuItem> menuItems)
    {
        var candidate = id;
        var suffix = 2;

        while (menuItems.Any(item => item.Id == candidate))
        {
            candidate = $"{id}-{suffix}";
            suffix++;
        }

        return candidate;
    }
}
