using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class CheckoutController : Controller
{
    private const string LastReceiptSessionKey = "LastReceiptId";

    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public CheckoutController(IMenuService menuService, IOrderService orderService, IUserService userService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _userService = userService;
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
        ViewBag.OrderTypes = OrderTypes;

        var customer = GetCurrentUser();
        return View(new CheckoutViewModel
        {
            CustomerName = customer?.FullName ?? string.Empty,
            Email = customer?.Email,
            Phone = customer?.Phone,
            DeliveryAddress = customer?.DefaultAddress,
            IsGuestCheckout = customer is null
        });
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

        if (!OrderTypes.Contains(checkout.OrderType))
        {
            ModelState.AddModelError(nameof(checkout.OrderType), "Select pickup or delivery.");
        }

        ValidatePaymentDetails(checkout);

        if (checkout.OrderType == "Delivery" && string.IsNullOrWhiteSpace(checkout.DeliveryAddress))
        {
            ModelState.AddModelError(nameof(checkout.DeliveryAddress), "Delivery address is required for delivery orders.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            ViewBag.PaymentMethods = PaymentMethods;
            ViewBag.OrderTypes = OrderTypes;
            return View(checkout);
        }

        var customer = GetCurrentUser();
        checkout.IsGuestCheckout = customer is null || checkout.IsGuestCheckout;
        var order = _orderService.CreateOrder(checkout, cart, customer);
        HttpContext.Session.SetString(LastReceiptSessionKey, order.Id);
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

        if (!CanViewOrder(order))
        {
            return Challenge();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reorder(string id)
    {
        var order = _orderService.GetOrder(id);

        if (order is null)
        {
            return NotFound();
        }

        if (!CanViewOrder(order))
        {
            return Challenge();
        }

        var restoredItems = order.Lines
            .Where(line => line.Quantity > 0 && _menuService.GetMenuItem(line.MenuItem.Id) is { IsAvailable: true })
            .GroupBy(line => line.MenuItem.Id)
            .ToDictionary(
                group => group.Key,
                group => new CartSessionItem
                {
                    Quantity = group.Sum(line => line.Quantity),
                    Notes = group.LastOrDefault(line => !string.IsNullOrWhiteSpace(line.Notes))?.Notes
                });

        if (restoredItems.Count == 0)
        {
            TempData["CartMessage"] = "None of the items from that receipt are currently available.";
            return RedirectToAction("Index", "Menu");
        }

        HttpContext.Session.SetString(CartController.CartSessionKey, JsonSerializer.Serialize(restoredItems));
        TempData["CartMessage"] = $"{restoredItems.Sum(item => item.Value.Quantity)} item(s) restored from receipt {order.Id}.";
        return RedirectToAction("Index", "Cart");
    }

    private static IReadOnlyList<string> PaymentMethods { get; } =
    [
        "Credit Card",
        "Debit Card",
        "PayPal",
        "E-Transfer"
    ];

    private static IReadOnlyList<string> OrderTypes { get; } =
    [
        "Pickup",
        "Delivery"
    ];

    private UserAccount? GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId) ? null : _userService.GetById(userId);
    }

    private bool CanViewOrder(Order order)
    {
        if (HttpContext.Session.GetString(LastReceiptSessionKey) == order.Id)
        {
            return true;
        }

        if (User.IsInRole(UserRoles.Administrator))
        {
            return true;
        }

        var customer = GetCurrentUser();

        if (customer is null)
        {
            return false;
        }

        return order.CustomerId == customer.Id ||
            order.Email?.Equals(customer.Email, StringComparison.OrdinalIgnoreCase) == true;
    }

    private void ValidatePaymentDetails(CheckoutViewModel checkout)
    {
        if (checkout.PaymentMethod is "Credit Card" or "Debit Card")
        {
            if (string.IsNullOrWhiteSpace(checkout.CardholderName))
            {
                ModelState.AddModelError(nameof(checkout.CardholderName), "Cardholder name is required.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardNumber))
            {
                ModelState.AddModelError(nameof(checkout.CardNumber), "Card number is required.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardExpiry))
            {
                ModelState.AddModelError(nameof(checkout.CardExpiry), "Expiry is required.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardCvv))
            {
                ModelState.AddModelError(nameof(checkout.CardCvv), "CVV is required.");
            }
        }

        if (checkout.PaymentMethod == "PayPal" && string.IsNullOrWhiteSpace(checkout.PayPalEmail))
        {
            ModelState.AddModelError(nameof(checkout.PayPalEmail), "PayPal email is required.");
        }

        if (checkout.PaymentMethod == "E-Transfer")
        {
            if (string.IsNullOrWhiteSpace(checkout.ETransferSenderName))
            {
                ModelState.AddModelError(nameof(checkout.ETransferSenderName), "Sender name is required.");
            }

            if (string.IsNullOrWhiteSpace(checkout.ETransferReference))
            {
                ModelState.AddModelError(nameof(checkout.ETransferReference), "Reference number is required.");
            }
        }
    }

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
