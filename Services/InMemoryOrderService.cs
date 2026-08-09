using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryOrderService : IOrderService
{
    private readonly List<Order> _orders = [];
    private readonly object _lock = new();

    public Order CreateOrder(CheckoutViewModel checkout, IReadOnlyList<CartLine> lines)
    {
        var order = new Order
        {
            Id = $"R-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
            CreatedAt = DateTimeOffset.UtcNow,
            CustomerName = checkout.CustomerName,
            Email = checkout.Email,
            Phone = checkout.Phone,
            PaymentMethod = checkout.PaymentMethod,
            IsGuestCheckout = checkout.IsGuestCheckout,
            Notes = checkout.Notes,
            Lines = lines.Select(line => new CartLine
            {
                MenuItem = line.MenuItem,
                Quantity = line.Quantity
            }).ToList()
        };

        lock (_lock)
        {
            _orders.Insert(0, order);
        }

        return order;
    }

    public IReadOnlyList<Order> GetOrders()
    {
        lock (_lock)
        {
            return _orders.ToList();
        }
    }

    public Order? GetOrder(string id)
    {
        lock (_lock)
        {
            return _orders.FirstOrDefault(order => order.Id == id);
        }
    }
}
