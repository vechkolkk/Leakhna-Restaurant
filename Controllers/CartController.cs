using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class CartController : Controller
{
    public const string CartSessionKey = "RestaurantCart";
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
    public IActionResult Add(string id, int quantity = 1, string? notes = null)
    {
        var menuItem = _menuService.GetMenuItem(id);

        if (menuItem is null || quantity < 1)
        {
            return NotFound();
        }

        var cart = GetCart();
        var existing = cart.GetValueOrDefault(id) ?? new CartSessionItem();
        existing.Quantity += quantity;

        if (!string.IsNullOrWhiteSpace(notes))
        {
            existing.Notes = notes.Trim();
        }

        cart[id] = existing;
        SaveCart(cart);

        TempData["CartMessage"] = $"{menuItem.Name} added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(string id, int quantity, string? notes = null)
    {
        var cart = GetCart();

        if (quantity <= 0)
        {
            cart.Remove(id);
        }
        else if (_menuService.GetMenuItem(id) is not null)
        {
            cart[id] = new CartSessionItem
            {
                Quantity = quantity,
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
            };
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(string id)
    {
        var cart = GetCart();
        var item = _menuService.GetMenuItem(id);
        cart.Remove(id);
        SaveCart(cart);

        if (item is not null)
        {
            TempData["CartMessage"] = $"{item.Name} removed from cart.";
        }

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
            .Where(entry => entry.MenuItem is not null && entry.Quantity.Quantity > 0)
            .Select(entry => new CartLine
            {
                MenuItem = entry.MenuItem!,
                Quantity = entry.Quantity.Quantity,
                Notes = entry.Quantity.Notes
            })
            .ToList();

        return new CartViewModel { Lines = lines };
    }

    internal void ClearCart()
    {
        SaveCart([]);
    }

    private Dictionary<string, CartSessionItem> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, CartSessionItem>>(json) ?? [];
        }
        catch (JsonException)
        {
            var legacyCart = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
            return legacyCart.ToDictionary(
                entry => entry.Key,
                entry => new CartSessionItem { Quantity = entry.Value });
        }
    }

    private void SaveCart(Dictionary<string, CartSessionItem> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }
}
