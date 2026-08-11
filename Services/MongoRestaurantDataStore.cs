using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestaurantApp.Models;
using RestaurantApp.Options;

namespace RestaurantApp.Services;

public class MongoRestaurantDataStore : IRestaurantDataStore
{
    private readonly IMongoCollection<MenuItem> _menuItems;
    private readonly IMongoCollection<Order> _orders;
    private readonly IMongoCollection<MenuReview> _reviews;
    private readonly IMongoCollection<UserAccount> _users;

    public MongoRestaurantDataStore(IOptions<MongoDbOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
        {
            throw new InvalidOperationException("MongoDb:ConnectionString is required when Persistence:Provider is MongoDb.");
        }

        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);

        _menuItems = database.GetCollection<MenuItem>(options.Value.MenuItemsCollection);
        _orders = database.GetCollection<Order>(options.Value.OrdersCollection);
        _reviews = database.GetCollection<MenuReview>(options.Value.ReviewsCollection);
        _users = database.GetCollection<UserAccount>(options.Value.UsersCollection);

        EnsureSeedData();
        EnsureIndexes();
    }

    public RestaurantDataSnapshot GetSnapshot()
    {
        var snapshot = new RestaurantDataSnapshot
        {
            MenuItems = _menuItems.Find(Builders<MenuItem>.Filter.Empty).ToList(),
            Users = _users.Find(Builders<UserAccount>.Filter.Empty).ToList(),
            Orders = _orders
                .Find(Builders<Order>.Filter.Empty)
                .SortByDescending(order => order.CreatedAt)
                .ToList(),
            Reviews = _reviews
                .Find(Builders<MenuReview>.Filter.Empty)
                .SortByDescending(review => review.CreatedAt)
                .ToList()
        };

        foreach (var item in snapshot.MenuItems)
        {
            SeedData.ApplyMissingMenuMetadata(item);
        }

        return snapshot;
    }

    public void SaveSnapshot(RestaurantDataSnapshot snapshot)
    {
        _menuItems.DeleteMany(Builders<MenuItem>.Filter.Empty);
        _users.DeleteMany(Builders<UserAccount>.Filter.Empty);
        _orders.DeleteMany(Builders<Order>.Filter.Empty);
        _reviews.DeleteMany(Builders<MenuReview>.Filter.Empty);

        if (snapshot.MenuItems.Count > 0)
        {
            _menuItems.InsertMany(snapshot.MenuItems);
        }

        if (snapshot.Users.Count > 0)
        {
            _users.InsertMany(snapshot.Users);
        }

        if (snapshot.Orders.Count > 0)
        {
            _orders.InsertMany(snapshot.Orders);
        }

        if (snapshot.Reviews.Count > 0)
        {
            _reviews.InsertMany(snapshot.Reviews);
        }
    }

    public UserAccount AddUser(UserAccount user)
    {
        _users.InsertOne(user);
        return user;
    }

    public UserAccount? UpdateUserProfile(string id, string fullName, string? phone, string? defaultAddress)
    {
        var update = Builders<UserAccount>.Update
            .Set(user => user.FullName, fullName)
            .Set(user => user.Phone, phone)
            .Set(user => user.DefaultAddress, defaultAddress);

        var result = _users.UpdateOne(user => user.Id == id, update);
        return result.MatchedCount == 0 ? null : _users.Find(user => user.Id == id).FirstOrDefault();
    }

    public bool UpdateUserFavorites(string id, IReadOnlyList<string> favoriteMenuItemIds)
    {
        var favorites = favoriteMenuItemIds
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var update = Builders<UserAccount>.Update
            .Set(user => user.FavoriteMenuItemIds, favorites);

        var result = _users.UpdateOne(user => user.Id == id, update);
        return result.ModifiedCount > 0 || result.MatchedCount > 0;
    }

    public bool UpdateUserPassword(string id, string passwordHash, string passwordSalt)
    {
        var update = Builders<UserAccount>.Update
            .Set(user => user.PasswordHash, passwordHash)
            .Set(user => user.PasswordSalt, passwordSalt);

        var result = _users.UpdateOne(user => user.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public Order AddOrder(Order order)
    {
        _orders.InsertOne(order);
        return order;
    }

    public bool UpdateOrderStatus(string id, string status, string paymentStatus)
    {
        var update = Builders<Order>.Update
            .Set(order => order.Status, status)
            .Set(order => order.PaymentStatus, paymentStatus);

        var result = _orders.UpdateOne(order => order.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public MenuReview AddReview(MenuReview review)
    {
        _reviews.InsertOne(review);
        return review;
    }

    public bool DeleteReview(string id)
    {
        var result = _reviews.DeleteOne(review => review.Id == id);
        return result.DeletedCount > 0;
    }

    private void EnsureSeedData()
    {
        if (_menuItems.CountDocuments(Builders<MenuItem>.Filter.Empty) == 0)
        {
            _menuItems.InsertMany(SeedData.MenuItems.Select(SeedData.CloneMenuItem));
        }

        if (_users.CountDocuments(Builders<UserAccount>.Filter.Empty) == 0)
        {
            _users.InsertMany(SeedData.Users);
        }
    }

    private void EnsureIndexes()
    {
        _users.Indexes.CreateOne(new CreateIndexModel<UserAccount>(
            Builders<UserAccount>.IndexKeys.Ascending(user => user.Email),
            new CreateIndexOptions { Unique = true }));

        _orders.Indexes.CreateOne(new CreateIndexModel<Order>(
            Builders<Order>.IndexKeys.Ascending(order => order.Id),
            new CreateIndexOptions { Unique = true }));

        _orders.Indexes.CreateOne(new CreateIndexModel<Order>(
            Builders<Order>.IndexKeys.Ascending(order => order.CustomerId)));

        _reviews.Indexes.CreateOne(new CreateIndexModel<MenuReview>(
            Builders<MenuReview>.IndexKeys.Ascending(review => review.MenuItemId)));

        _menuItems.Indexes.CreateOne(new CreateIndexModel<MenuItem>(
            Builders<MenuItem>.IndexKeys.Ascending(item => item.Category)));
    }

}
