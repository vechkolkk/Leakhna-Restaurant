using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IPromotionService
{
    IReadOnlyList<Promotion> GetPromotions();

    Promotion? GetPromotion(string? code);
}
