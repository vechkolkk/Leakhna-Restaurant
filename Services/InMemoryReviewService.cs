using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryReviewService : IReviewService
{
    private readonly List<MenuReview> _reviews = [];
    private readonly object _lock = new();

    public IReadOnlyList<MenuReview> GetReviews()
    {
        lock (_lock)
        {
            return _reviews
                .OrderByDescending(review => review.CreatedAt)
                .ToList();
        }
    }

    public IReadOnlyList<MenuReview> GetReviewsForMenuItem(string menuItemId)
    {
        lock (_lock)
        {
            return _reviews
                .Where(review => review.MenuItemId == menuItemId)
                .OrderByDescending(review => review.CreatedAt)
                .ToList();
        }
    }

    public IReadOnlyDictionary<string, ReviewSummary> GetReviewSummaries()
    {
        lock (_lock)
        {
            return _reviews
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
    }

    public MenuReview AddReview(string menuItemId, ReviewFormViewModel review, UserAccount? customer)
    {
        var menuReview = new MenuReview
        {
            Id = Guid.NewGuid().ToString("N"),
            MenuItemId = menuItemId,
            ReviewerName = string.IsNullOrWhiteSpace(customer?.FullName) ? review.ReviewerName.Trim() : customer.FullName,
            CustomerId = customer?.Id,
            Rating = review.Rating,
            Comment = review.Comment.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        lock (_lock)
        {
            _reviews.Insert(0, menuReview);
        }

        return menuReview;
    }

    public bool DeleteReview(string id)
    {
        lock (_lock)
        {
            var review = _reviews.FirstOrDefault(review => review.Id == id);

            if (review is null)
            {
                return false;
            }

            _reviews.Remove(review);
            return true;
        }
    }
}
