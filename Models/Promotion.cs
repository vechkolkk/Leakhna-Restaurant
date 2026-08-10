namespace RestaurantApp.Models;

public class Promotion
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal MinimumSubtotal { get; set; }

    public decimal PercentOff { get; set; }

    public decimal AmountOff { get; set; }

    public decimal MaxDiscount { get; set; }

    public decimal CalculateDiscount(decimal subtotal)
    {
        if (subtotal < MinimumSubtotal)
        {
            return 0;
        }

        var discount = AmountOff > 0 ? AmountOff : subtotal * PercentOff;

        if (MaxDiscount > 0)
        {
            discount = Math.Min(discount, MaxDiscount);
        }

        return Math.Round(Math.Min(discount, subtotal), 2, MidpointRounding.AwayFromZero);
    }
}
