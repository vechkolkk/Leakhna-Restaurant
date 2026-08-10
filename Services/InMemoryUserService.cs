using System.Security.Cryptography;
using System.Text;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class InMemoryUserService : IUserService
{
    private readonly List<UserAccount> _users = [];
    private readonly object _lock = new();

    public InMemoryUserService()
    {
        AddSeedUser("Administrator", "admin@leakhnas.local", "Admin123!", UserRoles.Administrator);
        AddSeedUser("Demo Customer", "customer@leakhnas.local", "Customer123!", UserRoles.Customer);
    }

    public UserAccount? Authenticate(string email, string password)
    {
        var user = GetByEmail(email);

        if (user is null)
        {
            return null;
        }

        return user.PasswordHash == HashPassword(password, user.PasswordSalt) ? user : null;
    }

    public UserAccount Register(RegisterViewModel registration)
    {
        lock (_lock)
        {
            if (_users.Any(user => user.Email.Equals(registration.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("An account already exists for this email.");
            }

            var salt = CreateSalt();
            var user = new UserAccount
            {
                Id = Guid.NewGuid().ToString("N"),
                FullName = registration.FullName,
                Email = registration.Email.Trim().ToLowerInvariant(),
                Phone = registration.Phone,
                DefaultAddress = registration.DefaultAddress,
                Role = UserRoles.Customer,
                PasswordSalt = salt,
                PasswordHash = HashPassword(registration.Password, salt)
            };

            _users.Add(user);
            return user;
        }
    }

    public UserAccount? GetById(string id)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }

    public UserAccount? GetByEmail(string email)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(user => user.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }

    public IReadOnlyList<UserAccount> GetUsers()
    {
        lock (_lock)
        {
            return _users.ToList();
        }
    }

    public UserAccount? UpdateProfile(string id, ProfileUpdateViewModel profile)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(user => user.Id == id);

            if (user is null)
            {
                return null;
            }

            user.FullName = profile.FullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(profile.Phone) ? null : profile.Phone.Trim();
            user.DefaultAddress = string.IsNullOrWhiteSpace(profile.DefaultAddress) ? null : profile.DefaultAddress.Trim();
            return user;
        }
    }

    public bool ChangePassword(string id, PasswordChangeViewModel password)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(user => user.Id == id);

            if (user is null || user.PasswordHash != HashPassword(password.CurrentPassword, user.PasswordSalt))
            {
                return false;
            }

            var salt = CreateSalt();
            user.PasswordSalt = salt;
            user.PasswordHash = HashPassword(password.NewPassword, salt);
            return true;
        }
    }

    private void AddSeedUser(string fullName, string email, string password, string role)
    {
        var salt = CreateSalt();
        _users.Add(new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            FullName = fullName,
            Email = email,
            Role = role,
            PasswordSalt = salt,
            PasswordHash = HashPassword(password, salt)
        });
    }

    private static string CreateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    }

    private static string HashPassword(string password, string salt)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{salt}:{password}"));
        return Convert.ToBase64String(bytes);
    }
}
