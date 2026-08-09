using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index(string? category)
    {
        var menuItems = _menuService.GetMenuItems();

        if (!string.IsNullOrWhiteSpace(category))
        {
            menuItems = menuItems
                .Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewBag.Categories = _menuService.GetCategories();
        ViewBag.SelectedCategory = category;

        return View(menuItems);
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
