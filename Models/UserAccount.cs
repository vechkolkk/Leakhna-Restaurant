namespace RestaurantApp.Models;

public class UserAccount
{
    public string Id { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? DefaultAddress { get; set; }

    public string Role { get; set; } = UserRoles.Customer;

    public string PasswordHash { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public static class UserRoles
{
    public const string Customer = "Customer";
    public const string Administrator = "Administrator";
}
