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
            var snapshot = JsonSerializer.Deserialize<RestaurantDataSnapshot>(json, JsonOptions) ?? CreateSeedSnapshot();

            if (ApplyMissingMenuMetadata(snapshot))
            {
                SaveSnapshot(snapshot);
            }

            return snapshot;
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

    public UserAccount? UpdateUserProfile(string id, string fullName, string? phone, string? defaultAddress)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            var user = snapshot.Users.FirstOrDefault(user => user.Id == id);

            if (user is null)
            {
                return null;
            }

            user.FullName = fullName;
            user.Phone = phone;
            user.DefaultAddress = defaultAddress;
            SaveSnapshot(snapshot);
            return user;
        }
    }

    public bool UpdateUserFavorites(string id, IReadOnlyList<string> favoriteMenuItemIds)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            var user = snapshot.Users.FirstOrDefault(user => user.Id == id);

            if (user is null)
            {
                return false;
            }

            user.FavoriteMenuItemIds = favoriteMenuItemIds
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            SaveSnapshot(snapshot);
            return true;
        }
    }

    public bool UpdateUserPassword(string id, string passwordHash, string passwordSalt)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            var user = snapshot.Users.FirstOrDefault(user => user.Id == id);

            if (user is null)
            {
                return false;
            }

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            SaveSnapshot(snapshot);
            return true;
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

    public bool UpdateOrderStatus(string id, string status, string paymentStatus)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            var order = snapshot.Orders.FirstOrDefault(order => order.Id == id);

            if (order is null)
            {
                return false;
            }

            order.Status = status;
            order.PaymentStatus = paymentStatus;
            SaveSnapshot(snapshot);
            return true;
        }
    }

    public MenuReview AddReview(MenuReview review)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            snapshot.Reviews.Insert(0, review);
            SaveSnapshot(snapshot);
            return review;
        }
    }

    public bool DeleteReview(string id)
    {
        lock (_lock)
        {
            var snapshot = GetSnapshot();
            var review = snapshot.Reviews.FirstOrDefault(review => review.Id == id);

            if (review is null)
            {
                return false;
            }

            snapshot.Reviews.Remove(review);
            SaveSnapshot(snapshot);
            return true;
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
            MenuItems = SeedData.MenuItems.Select(SeedData.CloneMenuItem).ToList(),
            Users = SeedData.Users,
            Orders = [],
            Reviews = []
        };
    }

    private static bool ApplyMissingMenuMetadata(RestaurantDataSnapshot snapshot)
    {
        var changed = false;

        foreach (var item in snapshot.MenuItems)
        {
            changed |= SeedData.ApplyMissingMenuMetadata(item);
        }

        return changed;
    }
}
