using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class CartController : Controller
{
    public const string CartSessionKey = "RestaurantCart";
    private const string PromoSessionKey = "RestaurantPromoCode";
    private readonly IMenuService _menuService;
    private readonly IPromotionService _promotionService;

    public CartController(IMenuService menuService, IPromotionService promotionService)
    {
        _menuService = menuService;
        _promotionService = promotionService;
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

        if (!menuItem.IsOrderable)
        {
            TempData["CartMessage"] = $"{menuItem.Name} is currently {menuItem.AvailabilityStatus.ToLowerInvariant()}.";
            return RedirectToAction("Index", "Menu", new { availableOnly = false });
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
        else if (_menuService.GetMenuItem(id) is { IsOrderable: true } menuItem)
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
        HttpContext.Session.Remove(PromoSessionKey);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplyPromo(string? promoCode)
    {
        var cart = BuildCartViewModel();

        if (cart.ItemCount == 0)
        {
            TempData["CartMessage"] = "Add at least one menu item before applying a promo.";
            return RedirectToAction(nameof(Index));
        }

        var promotion = _promotionService.GetPromotion(promoCode);

        if (promotion is null)
        {
            HttpContext.Session.Remove(PromoSessionKey);
            TempData["CartMessage"] = "That promo code was not found.";
            return RedirectToAction(nameof(Index));
        }

        var discount = promotion.CalculateDiscount(cart.Subtotal);

        if (discount <= 0)
        {
            HttpContext.Session.Remove(PromoSessionKey);
            TempData["CartMessage"] = $"{promotion.Code} requires a subtotal of at least {promotion.MinimumSubtotal.ToString("C")}.";
            return RedirectToAction(nameof(Index));
        }

        HttpContext.Session.SetString(PromoSessionKey, promotion.Code);
        TempData["CartMessage"] = $"{promotion.Code} applied: {promotion.Description}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemovePromo()
    {
        HttpContext.Session.Remove(PromoSessionKey);
        TempData["CartMessage"] = "Promo code removed.";
        return RedirectToAction(nameof(Index));
    }

    internal CartViewModel BuildCartViewModel()
    {
        var lines = GetCart()
            .Select(entry => new { MenuItem = _menuService.GetMenuItem(entry.Key), Quantity = entry.Value })
            .Where(entry => entry.MenuItem is { IsOrderable: true } && entry.Quantity.Quantity > 0)
            .Select(entry => new CartLine
            {
                MenuItem = entry.MenuItem!,
                Quantity = entry.Quantity.Quantity,
                Notes = entry.Quantity.Notes
            })
            .ToList();

        var promotion = _promotionService.GetPromotion(HttpContext.Session.GetString(PromoSessionKey));
        var viewModel = new CartViewModel { Lines = lines };

        if (promotion is not null && promotion.CalculateDiscount(viewModel.Subtotal) > 0)
        {
            viewModel.Promotion = promotion;
        }

        return viewModel;
    }

    internal void ClearCart()
    {
        SaveCart([]);
        HttpContext.Session.Remove(PromoSessionKey);
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
