using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;

namespace RestaurantApp.ViewComponents;

public class CartBadgeViewComponent : ViewComponent
{
    private const string CartSessionKey = "RestaurantCart";

    public IViewComponentResult Invoke()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return View(0);
        }

        try
        {
            var cart = JsonSerializer.Deserialize<Dictionary<string, CartSessionItem>>(json) ?? [];
            return View(cart.Values.Sum(item => item.Quantity));
        }
        catch (JsonException)
        {
            var legacyCart = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
            return View(legacyCart.Values.Sum());
        }
    }
}
