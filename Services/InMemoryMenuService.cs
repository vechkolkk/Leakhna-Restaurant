using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryMenuService : IMenuService
{
    private readonly IReadOnlyList<MenuItem> _menuItems =
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
        return _menuItems;
    }

    public MenuItem? GetMenuItem(string id)
    {
        return _menuItems.FirstOrDefault(item => item.Id == id);
    }

    public IReadOnlyList<string> GetCategories()
    {
        return _menuItems.Select(item => item.Category).Distinct().OrderBy(category => category).ToList();
    }
}
