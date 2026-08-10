using System.Security.Cryptography;
using System.Text;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public static class SeedData
{
    public static List<MenuItem> MenuItems { get; } =
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
            StockQuantity = 12,
            LowStockThreshold = 5,
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
            StockQuantity = 8,
            LowStockThreshold = 4,
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
            StockQuantity = 10,
            LowStockThreshold = 5,
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
            StockQuantity = 6,
            LowStockThreshold = 3,
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
            StockQuantity = 4,
            LowStockThreshold = 3,
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
            StockQuantity = 18,
            LowStockThreshold = 6,
            AccentClass = "accent-blue"
        }
    ];

    public static List<UserAccount> Users
    {
        get
        {
            return
            [
                CreateUser("Administrator", "admin@leakhnas.local", "Admin123!", UserRoles.Administrator),
                CreateUser("Demo Customer", "customer@leakhnas.local", "Customer123!", UserRoles.Customer)
            ];
        }
    }

    public static UserAccount CreateUser(string fullName, string email, string password, string role)
    {
        var salt = PasswordHasher.CreateSalt();

        return new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            FullName = fullName,
            Email = email.Trim().ToLowerInvariant(),
            Role = role,
            PasswordSalt = salt,
            PasswordHash = PasswordHasher.HashPassword(password, salt)
        };
    }
}

public static class PasswordHasher
{
    public static string CreateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    }

    public static string HashPassword(string password, string salt)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{salt}:{password}"));
        return Convert.ToBase64String(bytes);
    }
}
