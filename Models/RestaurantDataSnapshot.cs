namespace RestaurantApp.Models;

public class RestaurantDataSnapshot
{
    public List<MenuItem> MenuItems { get; set; } = [];

    public List<UserAccount> Users { get; set; } = [];

    public List<Order> Orders { get; set; } = [];

    public List<MenuReview> Reviews { get; set; } = [];
}
