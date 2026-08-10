namespace RestaurantApp.Models;

public static class OrderStatusOptions
{
    public static IReadOnlyList<string> All { get; } =
    [
        "Paid",
        "Awaiting Verification",
        "Preparing",
        "Ready for Pickup",
        "Out for Delivery",
        "Completed",
        "Cancelled"
    ];

    public static bool CanCustomerCancel(string status)
    {
        return status is "Paid" or "Awaiting Verification" or "Preparing";
    }
}
