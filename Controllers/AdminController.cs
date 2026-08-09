using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

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
}
