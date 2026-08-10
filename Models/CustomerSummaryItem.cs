namespace RestaurantApp.Models;

public class CustomerSummaryItem
{
    public string Id { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? DefaultAddress { get; set; }

    public int OrderCount { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTimeOffset? LastOrderAt { get; set; }
}
