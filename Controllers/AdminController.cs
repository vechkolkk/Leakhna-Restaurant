using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

[Authorize(Roles = UserRoles.Administrator)]
public class AdminController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;

    public AdminController(IMenuService menuService, IOrderService orderService)
    {
        _menuService = menuService;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View(new AdminDashboardViewModel
        {
            MenuItems = _menuService.GetMenuItems(),
            Orders = _orderService.GetOrders()
        });
    }

    public IActionResult CreateMenuItem()
    {
        ViewBag.AccentClasses = AccentClasses;
        return View("MenuItemForm", new MenuItemFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateMenuItem(MenuItemFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AccentClasses = AccentClasses;
            return View("MenuItemForm", form);
        }

        _menuService.AddMenuItem(form.ToMenuItem());
        TempData["AdminMessage"] = $"{form.Name} was added to the menu.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult EditMenuItem(string id)
    {
        var item = _menuService.GetMenuItem(id);

        if (item is null)
        {
            return NotFound();
        }

        ViewBag.AccentClasses = AccentClasses;
        return View("MenuItemForm", MenuItemFormViewModel.FromMenuItem(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditMenuItem(string id, MenuItemFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AccentClasses = AccentClasses;
            return View("MenuItemForm", form);
        }

        var updated = _menuService.UpdateMenuItem(form.ToMenuItem(id));

        if (!updated)
        {
            return NotFound();
        }

        TempData["AdminMessage"] = $"{form.Name} was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteMenuItem(string id)
    {
        var item = _menuService.GetMenuItem(id);

        if (item is null)
        {
            return NotFound();
        }

        _menuService.DeleteMenuItem(id);
        TempData["AdminMessage"] = $"{item.Name} was removed from the menu.";
        return RedirectToAction(nameof(Index));
    }

    private static IReadOnlyList<string> AccentClasses { get; } =
    [
        "accent-green",
        "accent-red",
        "accent-gold",
        "accent-teal",
        "accent-orange",
        "accent-blue"
    ];
}
