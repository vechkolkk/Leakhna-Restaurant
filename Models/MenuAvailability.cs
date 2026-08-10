namespace RestaurantApp.Models;

public static class MenuAvailability
{
    public const string Regular = "Regular";
    public const string Limited = "Limited";
    public const string SoldOut = "Sold out";
    public const string Unavailable = "Unavailable";

    public static IReadOnlyList<string> All { get; } =
    [
        Regular,
        Limited,
        SoldOut,
        Unavailable
    ];
}
