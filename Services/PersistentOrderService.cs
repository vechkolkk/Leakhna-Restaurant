using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class PersistentOrderService : IOrderService
{
    private readonly IRestaurantDataStore _dataStore;

    public PersistentOrderService(IRestaurantDataStore dataStore)
    {
        _dataStore = dataStore;
    }

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
            IsGuestCheckout = checkout.IsGuestCheckout,
            Status = checkout.PaymentMethod == "E-Transfer" ? "Awaiting Verification" : "Paid",
            Notes = checkout.Notes,
            Subtotal = cart.Subtotal,
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

        return _dataStore.AddOrder(order);
    }

    public IReadOnlyList<Order> GetOrders()
    {
        return _dataStore.GetSnapshot().Orders;
    }

    public IReadOnlyList<Order> GetOrdersForCustomer(string customerId, string email)
    {
        return GetOrders()
            .Where(order => order.CustomerId == customerId || order.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
    }

    public Order? GetOrder(string id)
    {
        return GetOrders().FirstOrDefault(order => order.Id == id);
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
}
