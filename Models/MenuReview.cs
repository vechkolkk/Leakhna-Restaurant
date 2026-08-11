using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models;

public class MenuReview
{
    public string Id { get; set; } = string.Empty;

    public string MenuItemId { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public string? CustomerId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    public string Comment { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
