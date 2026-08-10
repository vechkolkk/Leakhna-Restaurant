namespace RestaurantApp.Models;

public class SalesBreakdownItem
{
    public string Label { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Total { get; set; }
}
