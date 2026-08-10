namespace RestaurantApp.Models;

public class ProfileViewModel
{
    public required UserAccount User { get; set; }

    public ProfileUpdateViewModel Profile { get; set; } = new();

    public IReadOnlyList<Order> Orders { get; set; } = [];
}
