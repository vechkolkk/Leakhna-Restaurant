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

    public bool TryReserveStock(IReadOnlyList<CartLine> lines, out string? message)
    {
        var snapshot = _dataStore.GetSnapshot();

        foreach (var line in lines)
        {
            var item = snapshot.MenuItems.FirstOrDefault(existing => existing.Id == line.MenuItem.Id);

            if (item is null || !item.IsOrderable)
            {
                message = $"{line.MenuItem.Name} is no longer available.";
                return false;
            }

            if (line.Quantity > item.StockQuantity)
            {
                message = $"Only {item.StockQuantity} {item.Name} left in stock.";
                return false;
            }
        }

        foreach (var line in lines)
        {
            var item = snapshot.MenuItems.First(existing => existing.Id == line.MenuItem.Id);
            item.StockQuantity -= line.Quantity;
            item.IsAvailable = item.StockQuantity > 0 && item.IsAvailable;
        }

        _dataStore.SaveSnapshot(snapshot);
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
