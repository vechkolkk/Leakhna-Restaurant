namespace RestaurantApp.Models;

public class AdminCustomerDetailViewModel
{
    public required CustomerSummaryItem Customer { get; set; }

    public IReadOnlyList<Order> Orders { get; set; } = [];

    public decimal AverageOrderValue => Orders.Count == 0 ? 0 : Customer.TotalSpent / Orders.Count;
}
