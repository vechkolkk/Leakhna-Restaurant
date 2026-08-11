namespace RestaurantApp.Models;

public class MenuDetailsViewModel
{
    public required MenuItem Item { get; set; }

    public ReviewSummary ReviewSummary { get; set; } = new();

    public IReadOnlyList<MenuReview> Reviews { get; set; } = [];

    public ReviewFormViewModel ReviewForm { get; set; } = new();

    public bool CanReview { get; set; }

    public string ReviewGateMessage { get; set; } = string.Empty;
}
