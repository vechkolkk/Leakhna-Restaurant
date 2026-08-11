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
            DietaryTags = ["High protein", "Gluten friendly"],
            Allergens = ["Fish sauce"],
            EstimatedCalories = 640,
            EstimatedPrepMinutes = 18,
            Price = 15.99m,
            AvailabilityLevel = MenuAvailability.Regular,
            AccentClass = "accent-green"
        },
        new()
        {
            Id = "beef-lok-lak",
            Name = "Beef Lok Lak",
            Category = "Mains",
            Description = "Tender beef, fresh greens, tomato, onion, and lime pepper dipping sauce.",
            Ingredients = ["Beef", "Romaine", "Tomato", "Red onion", "Lime", "Black pepper"],
            DietaryTags = ["High protein", "Gluten friendly"],
            Allergens = ["Soy"],
            EstimatedCalories = 720,
            EstimatedPrepMinutes = 20,
            Price = 18.49m,
            AvailabilityLevel = MenuAvailability.Regular,
            AccentClass = "accent-red"
        },
        new()
        {
            Id = "vegetable-curry",
            Name = "Vegetable Curry",
            Category = "Mains",
            Description = "Coconut curry with seasonal vegetables, sweet potato, and steamed rice.",
            Ingredients = ["Coconut milk", "Sweet potato", "Bell pepper", "Green beans", "Rice"],
            DietaryTags = ["Vegetarian", "Gluten friendly"],
            Allergens = ["Coconut"],
            EstimatedCalories = 590,
            EstimatedPrepMinutes = 16,
            Price = 14.49m,
            AvailabilityLevel = MenuAvailability.Regular,
            AccentClass = "accent-gold"
        },
        new()
        {
            Id = "spring-rolls",
            Name = "Fresh Spring Rolls",
            Category = "Appetizers",
            Description = "Rice paper rolls filled with crisp vegetables, herbs, noodles, and peanut sauce.",
            Ingredients = ["Rice paper", "Rice noodles", "Lettuce", "Mint", "Carrot", "Peanut sauce"],
            DietaryTags = ["Vegetarian", "Light"],
            Allergens = ["Peanuts"],
            EstimatedCalories = 310,
            EstimatedPrepMinutes = 10,
            Price = 8.99m,
            AvailabilityLevel = MenuAvailability.Regular,
            AccentClass = "accent-teal"
        },
        new()
        {
            Id = "mango-sticky-rice",
            Name = "Mango Sticky Rice",
            Category = "Desserts",
            Description = "Sweet coconut sticky rice served with ripe mango and toasted sesame.",
            Ingredients = ["Mango", "Sticky rice", "Coconut milk", "Sesame"],
            DietaryTags = ["Vegetarian"],
            Allergens = ["Coconut", "Sesame"],
            EstimatedCalories = 430,
            EstimatedPrepMinutes = 8,
            Price = 7.99m,
            AvailabilityLevel = MenuAvailability.Limited,
            AccentClass = "accent-orange"
        },
        new()
        {
            Id = "iced-milk-tea",
            Name = "Iced Milk Tea",
            Category = "Drinks",
            Description = "Chilled black tea with milk, lightly sweetened and served over ice.",
            Ingredients = ["Black tea", "Milk", "Sugar", "Ice"],
            DietaryTags = ["Vegetarian"],
            Allergens = ["Milk"],
            EstimatedCalories = 180,
            EstimatedPrepMinutes = 4,
            Price = 4.99m,
            AvailabilityLevel = MenuAvailability.Regular,
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

    public static MenuItem CloneMenuItem(MenuItem item)
    {
        return new MenuItem
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Description = item.Description,
            Ingredients = item.Ingredients.ToList(),
            DietaryTags = item.DietaryTags.ToList(),
            Allergens = item.Allergens.ToList(),
            EstimatedCalories = item.EstimatedCalories,
            EstimatedPrepMinutes = item.EstimatedPrepMinutes,
            Price = item.Price,
            IsAvailable = item.IsAvailable,
            AvailabilityLevel = item.AvailabilityStatus,
            AccentClass = item.AccentClass
        };
    }

    public static bool ApplyMissingMenuMetadata(MenuItem item)
    {
        var seedItem = MenuItems.FirstOrDefault(seed => seed.Id == item.Id);

        if (seedItem is null)
        {
            return false;
        }

        var changed = false;

        if (item.DietaryTags.Count == 0 && seedItem.DietaryTags.Count > 0)
        {
            item.DietaryTags = seedItem.DietaryTags.ToList();
            changed = true;
        }

        if (item.Allergens.Count == 0 && seedItem.Allergens.Count > 0)
        {
            item.Allergens = seedItem.Allergens.ToList();
            changed = true;
        }

        if (item.EstimatedPrepMinutes <= 0)
        {
            item.EstimatedPrepMinutes = seedItem.EstimatedPrepMinutes;
            changed = true;
        }

        return changed;
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
