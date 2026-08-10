using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IRestaurantDataStore
{
    RestaurantDataSnapshot GetSnapshot();

    void SaveSnapshot(RestaurantDataSnapshot snapshot);

    UserAccount AddUser(UserAccount user);

    Order AddOrder(Order order);
}
