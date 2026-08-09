using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class HomeController : Controller
{
    private readonly IMenuService _menuService;

    public HomeController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index()
    {
        ViewBag.FeaturedItems = _menuService.GetMenuItems().Take(3).ToList();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
