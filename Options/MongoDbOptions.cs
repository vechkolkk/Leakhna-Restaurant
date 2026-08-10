namespace RestaurantApp.Options;

public class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = "LeakhnasRestaurant";

    public string UsersCollection { get; set; } = "users";

    public string MenuItemsCollection { get; set; } = "menuItems";

    public string OrdersCollection { get; set; } = "orders";
}
