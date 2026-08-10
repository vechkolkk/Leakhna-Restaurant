using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IOrderService
{
    Order CreateOrder(CheckoutViewModel checkout, CartViewModel cart, UserAccount? customer);

    IReadOnlyList<Order> GetOrders();

    IReadOnlyList<Order> GetOrdersForCustomer(string customerId, string email);

    Order? GetOrder(string id);
}
