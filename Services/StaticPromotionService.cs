using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class StaticPromotionService : IPromotionService
{
    private static readonly IReadOnlyList<Promotion> Promotions =
    [
        new()
        {
            Code = "WELCOME10",
            Description = "10% off orders $20 and up",
            MinimumSubtotal = 20m,
            PercentOff = 0.10m,
            MaxDiscount = 15m
        },
        new()
        {
            Code = "FAMILY5",
            Description = "$5 off orders $40 and up",
            MinimumSubtotal = 40m,
            AmountOff = 5m
        }
    ];

    public IReadOnlyList<Promotion> GetPromotions()
    {
        return Promotions;
    }

    public Promotion? GetPromotion(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return Promotions.FirstOrDefault(promotion =>
            promotion.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
