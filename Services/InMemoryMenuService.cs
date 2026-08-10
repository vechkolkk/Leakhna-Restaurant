using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryMenuService : IMenuService
{
    private readonly List<MenuItem> _menuItems =
    [
        new()
        {
            Id = "lemongrass-chicken",
            Name = "Lemongrass Chicken Bowl",
            Category = "Mains",
            Description = "Grilled lemongrass chicken over jasmine rice with pickled vegetables and herb sauce.",
            Ingredients = ["Chicken", "Lemongrass", "Jasmine rice", "Carrot", "Cucumber", "Cilantro"],
            EstimatedCalories = 640,
            Price = 15.99m,
            AccentClass = "accent-green"
        },
        new()
        {
            Id = "beef-lok-lak",
            Name = "Beef Lok Lak",
            Category = "Mains",
            Description = "Tender beef, fresh greens, tomato, onion, and lime pepper dipping sauce.",
            Ingredients = ["Beef", "Romaine", "Tomato", "Red onion", "Lime", "Black pepper"],
            EstimatedCalories = 720,
            Price = 18.49m,
            AccentClass = "accent-red"
        },
        new()
        {
            Id = "vegetable-curry",
            Name = "Vegetable Curry",
            Category = "Mains",
            Description = "Coconut curry with seasonal vegetables, sweet potato, and steamed rice.",
            Ingredients = ["Coconut milk", "Sweet potato", "Bell pepper", "Green beans", "Rice"],
            EstimatedCalories = 590,
            Price = 14.49m,
            AccentClass = "accent-gold"
        },
        new()
        {
            Id = "spring-rolls",
            Name = "Fresh Spring Rolls",
            Category = "Appetizers",
            Description = "Rice paper rolls filled with crisp vegetables, herbs, noodles, and peanut sauce.",
            Ingredients = ["Rice paper", "Rice noodles", "Lettuce", "Mint", "Carrot", "Peanut sauce"],
            EstimatedCalories = 310,
            Price = 8.99m,
            AccentClass = "accent-teal"
        },
        new()
        {
            Id = "mango-sticky-rice",
            Name = "Mango Sticky Rice",
            Category = "Desserts",
            Description = "Sweet coconut sticky rice served with ripe mango and toasted sesame.",
            Ingredients = ["Mango", "Sticky rice", "Coconut milk", "Sesame"],
            EstimatedCalories = 430,
            Price = 7.99m,
            AccentClass = "accent-orange"
        },
        new()
        {
            Id = "iced-milk-tea",
            Name = "Iced Milk Tea",
            Category = "Drinks",
            Description = "Chilled black tea with milk, lightly sweetened and served over ice.",
            Ingredients = ["Black tea", "Milk", "Sugar", "Ice"],
            EstimatedCalories = 180,
            Price = 4.99m,
            AccentClass = "accent-blue"
        }
    ];

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

    public bool TryReserveStock(IReadOnlyList<CartLine> lines, out string? message)
    {
        foreach (var line in lines)
        {
            var item = GetMenuItem(line.MenuItem.Id);

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
            var item = GetMenuItem(line.MenuItem.Id)!;
            item.StockQuantity -= line.Quantity;
            item.IsAvailable = item.StockQuantity > 0 && item.IsAvailable;
        }

        message = null;
        return true;
    }
}
