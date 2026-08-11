using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class PersistentReviewService : IReviewService
{
    private readonly IRestaurantDataStore _dataStore;

    public PersistentReviewService(IRestaurantDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<MenuReview> GetReviews()
    {
        return _dataStore.GetSnapshot().Reviews
            .OrderByDescending(review => review.CreatedAt)
            .ToList();
    }

    public IReadOnlyList<MenuReview> GetReviewsForMenuItem(string menuItemId)
    {
        return _dataStore.GetSnapshot().Reviews
            .Where(review => review.MenuItemId == menuItemId)
            .OrderByDescending(review => review.CreatedAt)
            .ToList();
    }

    public IReadOnlyDictionary<string, ReviewSummary> GetReviewSummaries()
    {
        return _dataStore.GetSnapshot().Reviews
            .GroupBy(review => review.MenuItemId)
            .ToDictionary(
                group => group.Key,
                group => new ReviewSummary
                {
                    MenuItemId = group.Key,
                    Count = group.Count(),
                    AverageRating = Math.Round((decimal)group.Average(review => review.Rating), 1)
                });
    }

    public MenuReview AddReview(string menuItemId, ReviewFormViewModel review, UserAccount? customer)
    {
        return _dataStore.AddReview(new MenuReview
        {
            Id = Guid.NewGuid().ToString("N"),
            MenuItemId = menuItemId,
            ReviewerName = string.IsNullOrWhiteSpace(customer?.FullName) ? review.ReviewerName.Trim() : customer.FullName,
            CustomerId = customer?.Id,
            Rating = review.Rating,
            Comment = review.Comment.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    public bool DeleteReview(string id)
    {
        return _dataStore.DeleteReview(id);
    }
}
