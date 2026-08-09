using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IOrderService
{
    Order CreateOrder(CheckoutViewModel checkout, IReadOnlyList<CartLine> lines);

    IReadOnlyList<Order> GetOrders();

    Order? GetOrder(string id);
}
