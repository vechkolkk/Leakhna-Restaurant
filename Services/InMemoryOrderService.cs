using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryOrderService : IOrderService
{
    private readonly List<Order> _orders = [];
    private readonly object _lock = new();

    public Order CreateOrder(CheckoutViewModel checkout, CartViewModel cart, UserAccount? customer)
    {
        var order = new Order
        {
            Id = $"R-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
            CreatedAt = DateTimeOffset.UtcNow,
            CustomerName = checkout.CustomerName,
            Email = checkout.Email,
            Phone = checkout.Phone,
            CustomerId = customer?.Id,
            PaymentMethod = checkout.PaymentMethod,
            PaymentStatus = checkout.PaymentMethod == "E-Transfer" ? "Pending Verification" : "Paid",
            PaymentSummary = BuildPaymentSummary(checkout),
            OrderType = checkout.OrderType,
            DeliveryAddress = checkout.DeliveryAddress,
            FulfillmentTiming = checkout.FulfillmentTiming,
            RequestedFulfillmentAt = checkout.RequestedFulfillmentAt,
            IsGuestCheckout = checkout.IsGuestCheckout,
            Status = checkout.PaymentMethod == "E-Transfer" ? "Awaiting Verification" : "Paid",
            Notes = checkout.Notes,
            Subtotal = cart.Subtotal,
            DiscountCode = cart.PromoCode,
            DiscountLabel = cart.Promotion?.Description,
            Discount = cart.Discount,
            TaxRate = cart.TaxRate,
            Tax = cart.Tax,
            Total = cart.Total,
            Lines = cart.Lines.Select(line => new CartLine
            {
                MenuItem = line.MenuItem,
                Quantity = line.Quantity,
                Notes = line.Notes
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

    public IReadOnlyList<Order> GetOrdersForCustomer(string customerId, string email)
    {
        lock (_lock)
        {
            return _orders
                .Where(order => order.CustomerId == customerId || order.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }
    }

    public Order? GetOrder(string id)
    {
        lock (_lock)
        {
            return _orders.FirstOrDefault(order => order.Id == id);
        }
    }

    public bool UpdateOrderStatus(string id, string status)
    {
        if (!OrderStatusOptions.All.Contains(status))
        {
            return false;
        }

        lock (_lock)
        {
            var order = _orders.FirstOrDefault(order => order.Id == id);

            if (order is null)
            {
                return false;
            }

            order.Status = status;
            order.PaymentStatus = GetPaymentStatus(status);
            return true;
        }
    }

    private static string BuildPaymentSummary(CheckoutViewModel checkout)
    {
        return checkout.PaymentMethod switch
        {
            "Credit Card" or "Debit Card" => string.IsNullOrWhiteSpace(checkout.CardNumber)
                ? "Card payment details captured for demo"
                : $"Card ending in {checkout.CardNumber[^Math.Min(4, checkout.CardNumber.Length)..]}",
            "PayPal" => $"PayPal account {checkout.PayPalEmail}",
            "E-Transfer" => $"Reference {checkout.ETransferReference} from {checkout.ETransferSenderName}",
            _ => checkout.PaymentMethod
        };
    }

    private static string GetPaymentStatus(string status)
    {
        return status switch
        {
            "Awaiting Verification" => "Pending Verification",
            "Cancelled" => "Cancelled",
            _ => "Paid"
        };
    }
}
