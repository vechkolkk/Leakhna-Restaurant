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

    public static IReadOnlyList<string> GetCustomerTimeline(string orderType)
    {
        return orderType == "Delivery"
            ? ["Paid", "Preparing", "Out for Delivery", "Completed"]
            : ["Paid", "Preparing", "Ready for Pickup", "Completed"];
    }

    public static bool IsTimelineStepComplete(string currentStatus, string step, string orderType)
    {
        if (currentStatus == "Cancelled")
        {
            return false;
        }

        if (currentStatus == "Awaiting Verification")
        {
            return step == "Paid";
        }

        var timeline = GetCustomerTimeline(orderType).ToList();
        var currentIndex = timeline.FindIndex(status => status == currentStatus);
        var stepIndex = timeline.FindIndex(status => status == step);

        return currentIndex >= 0 && stepIndex >= 0 && stepIndex <= currentIndex;
    }
}
