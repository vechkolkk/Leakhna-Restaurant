using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index(string? category, string? search, bool availableOnly = true)
    {
        var menuItems = _menuService.GetMenuItems().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            menuItems = menuItems
                .Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            menuItems = menuItems.Where(item =>
                item.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Ingredients.Any(ingredient => ingredient.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (availableOnly)
        {
            menuItems = menuItems.Where(item => item.IsAvailable);
        }

        return View(new MenuIndexViewModel
        {
            Items = menuItems.OrderBy(item => item.Category).ThenBy(item => item.Name).ToList(),
            Categories = _menuService.GetCategories(),
            Category = category,
            Search = search,
            AvailableOnly = availableOnly
        });
    }

    public IActionResult Details(string id)
    {
        var menuItem = _menuService.GetMenuItem(id);

        if (menuItem is null)
        {
            return NotFound();
        }

        return View(menuItem);
    }
}
