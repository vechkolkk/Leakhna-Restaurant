using System.Text;
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
    private readonly IUserService _userService;

    public AdminController(IMenuService menuService, IOrderService orderService, IUserService userService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _userService = userService;
    }

    public IActionResult Index(DateTime? dateFrom, DateTime? dateTo, string? paymentMethod, string? orderStatus, string? search)
    {
        var allOrders = _orderService.GetOrders();
        var filteredOrders = ApplyOrderFilters(allOrders, dateFrom, dateTo, paymentMethod, orderStatus, search);

        return View(new AdminDashboardViewModel
        {
            MenuItems = _menuService.GetMenuItems(),
            Orders = filteredOrders,
            Customers = BuildCustomerSummaries(_userService.GetUsers(), allOrders),
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
            StatusBreakdown = BuildBreakdown(filteredOrders, order => order.Status),
            TopDishes = BuildTopDishes(filteredOrders)
        });
    }

    public IActionResult Customer(string id)
    {
        var user = _userService.GetById(id);

        if (user is null || user.Role != UserRoles.Customer)
        {
            return NotFound();
        }

        var customerOrders = _orderService
            .GetOrdersForCustomer(user.Id, user.Email)
            .OrderByDescending(order => order.CreatedAt)
            .ToList();

        return View(new AdminCustomerDetailViewModel
        {
            Customer = BuildCustomerSummaries([user], customerOrders).Single(),
            Orders = customerOrders
        });
    }

    public IActionResult ExportSales(DateTime? dateFrom, DateTime? dateTo, string? paymentMethod, string? orderStatus, string? search)
    {
        var orders = ApplyOrderFilters(_orderService.GetOrders(), dateFrom, dateTo, paymentMethod, orderStatus, search);
        var csv = BuildSalesCsv(orders);
        var fileName = $"leakhnas-sales-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";

        return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
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

    private static IReadOnlyList<CustomerSummaryItem> BuildCustomerSummaries(
        IReadOnlyList<UserAccount> users,
        IReadOnlyList<Order> orders)
    {
        return users
            .Where(user => user.Role == UserRoles.Customer)
            .Select(user =>
            {
                var customerOrders = orders
                    .Where(order => order.CustomerId == user.Id || order.Email?.Equals(user.Email, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();

                return new CustomerSummaryItem
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    DefaultAddress = user.DefaultAddress,
                    OrderCount = customerOrders.Count,
                    TotalSpent = customerOrders.Sum(order => order.Total),
                    LastOrderAt = customerOrders.Count == 0 ? null : customerOrders.Max(order => order.CreatedAt)
                };
            })
            .OrderByDescending(customer => customer.LastOrderAt)
            .ThenBy(customer => customer.FullName)
            .ToList();
    }

    private static IReadOnlyList<TopSellingDishItem> BuildTopDishes(IReadOnlyList<Order> orders)
    {
        return orders
            .SelectMany(order => order.Lines)
            .GroupBy(line => line.MenuItem.Id)
            .Select(group =>
            {
                var firstLine = group.First();
                return new TopSellingDishItem
                {
                    MenuItemId = firstLine.MenuItem.Id,
                    Name = firstLine.MenuItem.Name,
                    Category = firstLine.MenuItem.Category,
                    QuantitySold = group.Sum(line => line.Quantity),
                    Revenue = group.Sum(line => line.LineTotal)
                };
            })
            .OrderByDescending(item => item.QuantitySold)
            .ThenByDescending(item => item.Revenue)
            .Take(5)
            .ToList();
    }

    private static string BuildSalesCsv(IReadOnlyList<Order> orders)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Receipt,Date,Customer,Email,Phone,Payment Method,Payment Status,Order Status,Order Type,Subtotal,HST,Total,Items,Notes");

        foreach (var order in orders)
        {
            var items = string.Join("; ", order.Lines.Select(line => $"{line.Quantity} x {line.MenuItem.Name}"));
            csv.AppendLine(string.Join(
                ",",
                Csv(order.Id),
                Csv(order.CreatedAt.LocalDateTime.ToString("yyyy-MM-dd HH:mm")),
                Csv(order.CustomerName),
                Csv(order.Email),
                Csv(order.Phone),
                Csv(order.PaymentMethod),
                Csv(order.PaymentStatus),
                Csv(order.Status),
                Csv(order.OrderType),
                Csv(order.Subtotal.ToString("0.00")),
                Csv(order.Tax.ToString("0.00")),
                Csv(order.Total.ToString("0.00")),
                Csv(items),
                Csv(order.Notes)));
        }

        return csv.ToString();
    }

    private static string Csv(string? value)
    {
        var escaped = (value ?? string.Empty).Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
