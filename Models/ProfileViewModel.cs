namespace RestaurantApp.Models;

public class ProfileViewModel
{
    public required UserAccount User { get; set; }

    public IReadOnlyList<Order> Orders { get; set; } = [];
}
