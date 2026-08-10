using System.Text.Json;
using Microsoft.Extensions.Options;
using RestaurantApp.Models;
using RestaurantApp.Options;

namespace RestaurantApp.Services;

public class JsonRestaurantDataStore : IRestaurantDataStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _dataPath;
    private readonly object _lock = new();

    public JsonRestaurantDataStore(IOptions<PersistenceOptions> options, IWebHostEnvironment environment)
    {
        _dataPath = Path.IsPathRooted(options.Value.JsonDataPath)
            ? options.Value.JsonDataPath
            : Path.Combine(environment.ContentRootPath, options.Value.JsonDataPath);
    }

    public RestaurantDataSnapshot GetSnapshot()
    {
        lock (_lock)
        {
            EnsureDataFile();
            var json = File.ReadAllText(_dataPath);
            return JsonSerializer.Deserialize<RestaurantDataSnapshot>(json, JsonOptions) ?? CreateSeedSnapshot();
        }
    }

    public void SaveSnapshot(RestaurantDataSnapshot snapshot)
    {
        lock (_lock)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
            File.WriteAllText(_dataPath, JsonSerializer.Serialize(snapshot, JsonOptions));
        }
    }

    public UserAccount AddUser(UserAccount user)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            snapshot.Users.Add(user);
            SaveSnapshot(snapshot);
            return user;
        }
    }

    public Order AddOrder(Order order)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            snapshot.Orders.Insert(0, order);
            SaveSnapshot(snapshot);
            return order;
        }
    }

    private void EnsureDataFile()
    {
        if (File.Exists(_dataPath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
        File.WriteAllText(_dataPath, JsonSerializer.Serialize(CreateSeedSnapshot(), JsonOptions));
    }

    private static RestaurantDataSnapshot CreateSeedSnapshot()
    {
        return new RestaurantDataSnapshot
        {
            MenuItems = SeedData.MenuItems.Select(CloneMenuItem).ToList(),
            Users = SeedData.Users,
            Orders = []
        };
    }

    private static MenuItem CloneMenuItem(MenuItem item)
    {
        return new MenuItem
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Description = item.Description,
            Ingredients = item.Ingredients.ToList(),
            EstimatedCalories = item.EstimatedCalories,
            Price = item.Price,
            IsAvailable = item.IsAvailable,
            AccentClass = item.AccentClass
        };
    }
}
