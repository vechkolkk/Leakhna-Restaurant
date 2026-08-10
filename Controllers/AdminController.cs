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

    public IActionResult Index(DateTime? dateFrom, DateTime? dateTo, string? paymentMethod, string? orderStatus, string? search)
    {
        var allOrders = _orderService.GetOrders();
        var filteredOrders = ApplyOrderFilters(allOrders, dateFrom, dateTo, paymentMethod, orderStatus, search);

        return View(new AdminDashboardViewModel
        {
            MenuItems = _menuService.GetMenuItems(),
            Orders = filteredOrders,
            DateFrom = dateFrom,
            DateTo = dateTo,
            PaymentMethod = paymentMethod,
            OrderStatus = orderStatus,
            Search = search,
            PaymentMethods = allOrders.Select(order => order.PaymentMethod).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value).ToList(),
            OrderStatuses = OrderStatusOptions.All
                .Concat(allOrders.Select(order => order.Status).Where(value => !string.IsNullOrWhiteSpace(value)))
                .Distinct()
                .OrderBy(value => value)
                .ToList(),
            AvailableOrderStatuses = OrderStatusOptions.All,
            PaymentBreakdown = BuildBreakdown(filteredOrders, order => order.PaymentMethod),
            StatusBreakdown = BuildBreakdown(filteredOrders, order => order.Status)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderStatus(string orderId, string status)
    {
        if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(status))
        {
            TempData["AdminMessage"] = "Choose a receipt and status before updating.";
            return RedirectToAction(nameof(Index));
        }

        var updated = _orderService.UpdateOrderStatus(orderId, status);
        TempData["AdminMessage"] = updated
            ? $"{orderId} is now {status}."
            : $"Could not update {orderId}.";

        return RedirectToAction(nameof(Index));
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

    private static IReadOnlyList<Order> ApplyOrderFilters(
        IReadOnlyList<Order> orders,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? paymentMethod,
        string? orderStatus,
        string? search)
    {
        var query = orders.AsEnumerable();

        if (dateFrom.HasValue)
        {
            query = query.Where(order => order.CreatedAt.LocalDateTime.Date >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(order => order.CreatedAt.LocalDateTime.Date <= dateTo.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod))
        {
            query = query.Where(order => order.PaymentMethod.Equals(paymentMethod, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(orderStatus))
        {
            query = query.Where(order => order.Status.Equals(orderStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(order =>
                order.Id.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                order.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                order.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
        }

        return query.OrderByDescending(order => order.CreatedAt).ToList();
    }

    private static IReadOnlyList<SalesBreakdownItem> BuildBreakdown(IReadOnlyList<Order> orders, Func<Order, string> labelSelector)
    {
        return orders
            .GroupBy(labelSelector)
            .Select(group => new SalesBreakdownItem
            {
                Label = string.IsNullOrWhiteSpace(group.Key) ? "Unknown" : group.Key,
                Count = group.Count(),
                Total = group.Sum(order => order.Total)
            })
            .OrderByDescending(item => item.Total)
            .ToList();
    }
}
