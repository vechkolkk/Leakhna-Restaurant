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
    private readonly IPromotionService _promotionService;
    private readonly IUserService _userService;

    public CheckoutController(
        IMenuService menuService,
        IOrderService orderService,
        IPromotionService promotionService,
        IUserService userService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _promotionService = promotionService;
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
            ModelState.AddModelError(nameof(checkout.PaymentMethod), "Choose one of the available payment methods.");
        }

        if (!OrderTypes.Contains(checkout.OrderType))
        {
            ModelState.AddModelError(nameof(checkout.OrderType), "Choose pickup or delivery.");
        }

        if (!FulfillmentTimings.Contains(checkout.FulfillmentTiming))
        {
            ModelState.AddModelError(nameof(checkout.FulfillmentTiming), "Choose ASAP or schedule the order for later.");
        }

        ValidatePaymentDetails(checkout);
        ValidateFulfillmentTiming(checkout);

        if (checkout.OrderType == "Delivery" && string.IsNullOrWhiteSpace(checkout.DeliveryAddress))
        {
            ModelState.AddModelError(nameof(checkout.DeliveryAddress), "Enter the delivery address for this order.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            ViewBag.PaymentMethods = PaymentMethods;
            ViewBag.OrderTypes = OrderTypes;
            return View(checkout);
        }

        if (!_menuService.ValidateAvailability(cart.Lines, out var availabilityMessage))
        {
            TempData["CartMessage"] = availabilityMessage ?? "One or more cart items are no longer available.";
            return RedirectToAction("Index", "Cart");
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
            .Where(line => line.Quantity > 0 && _menuService.GetMenuItem(line.MenuItem.Id) is { IsOrderable: true })
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(string id)
    {
        var order = _orderService.GetOrder(id);

        if (order is null)
        {
            return NotFound();
        }

        if (!CanViewOrder(order) || !CanCurrentUserCancel(order))
        {
            return Forbid();
        }

        if (!OrderStatusOptions.CanCustomerCancel(order.Status))
        {
            TempData["ReceiptMessage"] = $"Receipt {order.Id} can no longer be cancelled.";
            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        _orderService.UpdateOrderStatus(order.Id, "Cancelled");
        TempData["ReceiptMessage"] = $"Receipt {order.Id} was cancelled.";
        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
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

    private static IReadOnlyList<string> FulfillmentTimings { get; } =
    [
        "ASAP",
        "Scheduled"
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

    private bool CanCurrentUserCancel(Order order)
    {
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
                ModelState.AddModelError(nameof(checkout.CardholderName), "Enter the name shown on the card.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardNumber))
            {
                ModelState.AddModelError(nameof(checkout.CardNumber), "Enter the card number for this demo payment.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardExpiry))
            {
                ModelState.AddModelError(nameof(checkout.CardExpiry), "Enter the card expiry as MM/YY.");
            }

            if (string.IsNullOrWhiteSpace(checkout.CardCvv))
            {
                ModelState.AddModelError(nameof(checkout.CardCvv), "Enter the 3 or 4 digit CVV.");
            }
        }

        if (checkout.PaymentMethod == "PayPal" && string.IsNullOrWhiteSpace(checkout.PayPalEmail))
        {
            ModelState.AddModelError(nameof(checkout.PayPalEmail), "Enter the PayPal email for this demo payment.");
        }

        if (checkout.PaymentMethod == "E-Transfer")
        {
            if (string.IsNullOrWhiteSpace(checkout.ETransferSenderName))
            {
                ModelState.AddModelError(nameof(checkout.ETransferSenderName), "Enter the name the e-transfer will be sent from.");
            }

            if (string.IsNullOrWhiteSpace(checkout.ETransferReference))
            {
                ModelState.AddModelError(nameof(checkout.ETransferReference), "Enter the e-transfer confirmation or reference number.");
            }
        }
    }

    private void ValidateFulfillmentTiming(CheckoutViewModel checkout)
    {
        if (checkout.FulfillmentTiming == "ASAP")
        {
            checkout.RequestedFulfillmentAt = null;
            return;
        }

        if (!checkout.RequestedFulfillmentAt.HasValue)
        {
            ModelState.AddModelError(nameof(checkout.RequestedFulfillmentAt), "Choose the pickup or delivery time.");
            return;
        }

        if (checkout.RequestedFulfillmentAt.Value < DateTime.Now.AddMinutes(15))
        {
            ModelState.AddModelError(nameof(checkout.RequestedFulfillmentAt), "Choose a time at least 15 minutes from now so the kitchen can prepare it.");
        }
    }

    private CartViewModel BuildCartViewModel()
    {
        var cartController = new CartController(_menuService, _promotionService)
        {
            ControllerContext = ControllerContext
        };

        return cartController.BuildCartViewModel();
    }

    private void ClearCart()
    {
        var cartController = new CartController(_menuService, _promotionService)
        {
            ControllerContext = ControllerContext
        };

        cartController.ClearCart();
    }
}
