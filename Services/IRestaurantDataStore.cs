using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IRestaurantDataStore
{
    RestaurantDataSnapshot GetSnapshot();

    void SaveSnapshot(RestaurantDataSnapshot snapshot);

    UserAccount AddUser(UserAccount user);

    UserAccount? UpdateUserProfile(string id, string fullName, string? phone, string? defaultAddress);

    Order AddOrder(Order order);

    bool UpdateOrderStatus(string id, string status, string paymentStatus);
}
