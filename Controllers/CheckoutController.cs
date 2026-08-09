using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class CheckoutController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;

    public CheckoutController(IMenuService menuService, IOrderService orderService)
    {
        _menuService = menuService;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        var cart = BuildCartViewModel();

        if (cart.ItemCount == 0)
        {
            TempData["CartMessage"] = "Add at least one menu item before checkout.";
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Cart = cart;
        ViewBag.PaymentMethods = PaymentMethods;
        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CheckoutViewModel checkout)
    {
        var cart = BuildCartViewModel();

        if (cart.ItemCount == 0)
        {
            TempData["CartMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (!PaymentMethods.Contains(checkout.PaymentMethod))
        {
            ModelState.AddModelError(nameof(checkout.PaymentMethod), "Select a supported payment method.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            ViewBag.PaymentMethods = PaymentMethods;
            return View(checkout);
        }

        var order = _orderService.CreateOrder(checkout, cart.Lines);
        ClearCart();

        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
    }

    public IActionResult Confirmation(string id)
    {
        var order = _orderService.GetOrder(id);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    private static IReadOnlyList<string> PaymentMethods { get; } =
    [
        "Credit Card",
        "Debit Card",
        "PayPal",
        "E-Transfer"
    ];

    private CartViewModel BuildCartViewModel()
    {
        var cartController = new CartController(_menuService)
        {
            ControllerContext = ControllerContext
        };

        return cartController.BuildCartViewModel();
    }

    private void ClearCart()
    {
        var cartController = new CartController(_menuService)
        {
            ControllerContext = ControllerContext
        };

        cartController.ClearCart();
    }
}
