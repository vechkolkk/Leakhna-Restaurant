namespace RestaurantApp.Models;

public class AdminDashboardViewModel
{
    public IReadOnlyList<MenuItem> MenuItems { get; set; } = [];

    public IReadOnlyList<Order> Orders { get; set; } = [];

    public decimal TotalSales => Orders.Sum(order => order.Total);

    public decimal TotalTax => Orders.Sum(order => order.Tax);

    public int ReceiptCount => Orders.Count;
}
