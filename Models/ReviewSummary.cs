namespace RestaurantApp.Models;

public class ReviewSummary
{
    public string MenuItemId { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal AverageRating { get; set; }

    public string DisplayText => Count == 0
        ? "No reviews yet"
        : $"{AverageRating:0.0} / 5 ({Count})";
}
