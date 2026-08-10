namespace RestaurantApp.Models;

public class OrderQueueGroup
{
    public string Status { get; set; } = string.Empty;

    public IReadOnlyList<Order> Orders { get; set; } = [];
}
