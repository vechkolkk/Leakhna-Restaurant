namespace RestaurantApp.Models;

public class AdminDashboardViewModel
{
    public IReadOnlyList<MenuItem> MenuItems { get; set; } = [];

    public IReadOnlyList<Order> Orders { get; set; } = [];

    public IReadOnlyList<CustomerSummaryItem> Customers { get; set; } = [];

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? PaymentMethod { get; set; }

    public string? OrderStatus { get; set; }

    public string? Search { get; set; }

    public IReadOnlyList<string> PaymentMethods { get; set; } = [];

    public IReadOnlyList<string> OrderStatuses { get; set; } = [];

    public IReadOnlyList<string> AvailableOrderStatuses { get; set; } = OrderStatusOptions.All;

    public IReadOnlyList<SalesBreakdownItem> PaymentBreakdown { get; set; } = [];

    public IReadOnlyList<SalesBreakdownItem> StatusBreakdown { get; set; } = [];

    public IReadOnlyList<TopSellingDishItem> TopDishes { get; set; } = [];

    public decimal TotalSales => Orders.Sum(order => order.Total);

    public decimal TotalTax => Orders.Sum(order => order.Tax);

    public decimal AverageOrderValue => ReceiptCount == 0 ? 0 : TotalSales / ReceiptCount;

    public int ReceiptCount => Orders.Count;

    public int CustomerCount => Customers.Count;
}
