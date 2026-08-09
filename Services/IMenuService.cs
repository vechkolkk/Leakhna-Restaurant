using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IMenuService
{
    IReadOnlyList<MenuItem> GetMenuItems();

    MenuItem? GetMenuItem(string id);

    IReadOnlyList<string> GetCategories();
}
