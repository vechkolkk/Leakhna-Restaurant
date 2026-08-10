using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IMenuService
{
    IReadOnlyList<MenuItem> GetMenuItems();

    MenuItem? GetMenuItem(string id);

    IReadOnlyList<string> GetCategories();

    MenuItem AddMenuItem(MenuItem item);

    bool UpdateMenuItem(MenuItem item);

    bool DeleteMenuItem(string id);

    bool TryReserveStock(IReadOnlyList<CartLine> lines, out string? message);
}
