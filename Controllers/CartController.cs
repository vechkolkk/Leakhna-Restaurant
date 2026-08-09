using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "RestaurantCart";
    private readonly IMenuService _menuService;

    public CartController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index()
    {
        return View(BuildCartViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(string id, int quantity = 1)
    {
        var menuItem = _menuService.GetMenuItem(id);

        if (menuItem is null || quantity < 1)
        {
            return NotFound();
        }

        var cart = GetCart();
        cart[id] = cart.GetValueOrDefault(id) + quantity;
        SaveCart(cart);

        TempData["CartMessage"] = $"{menuItem.Name} added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(string id, int quantity)
    {
        var cart = GetCart();

        if (quantity <= 0)
        {
            cart.Remove(id);
        }
        else if (_menuService.GetMenuItem(id) is not null)
        {
            cart[id] = quantity;
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        SaveCart([]);
        return RedirectToAction(nameof(Index));
    }

    internal CartViewModel BuildCartViewModel()
    {
        var lines = GetCart()
            .Select(entry => new { MenuItem = _menuService.GetMenuItem(entry.Key), Quantity = entry.Value })
            .Where(entry => entry.MenuItem is not null && entry.Quantity > 0)
            .Select(entry => new CartLine { MenuItem = entry.MenuItem!, Quantity = entry.Quantity })
            .ToList();

        return new CartViewModel { Lines = lines };
    }

    internal void ClearCart()
    {
        SaveCart([]);
    }

    private Dictionary<string, int> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
    }

    private void SaveCart(Dictionary<string, int> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }
}
