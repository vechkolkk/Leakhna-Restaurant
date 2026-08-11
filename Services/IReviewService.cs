using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IReviewService
{
    IReadOnlyList<MenuReview> GetReviews();

    IReadOnlyList<MenuReview> GetReviewsForMenuItem(string menuItemId);

    IReadOnlyDictionary<string, ReviewSummary> GetReviewSummaries();

    MenuReview AddReview(string menuItemId, ReviewFormViewModel review, UserAccount? customer);

    bool DeleteReview(string id);
}
