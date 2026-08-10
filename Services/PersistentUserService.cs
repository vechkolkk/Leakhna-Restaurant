using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class PersistentUserService : IUserService
{
    private readonly IRestaurantDataStore _dataStore;

    public PersistentUserService(IRestaurantDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public UserAccount? Authenticate(string email, string password)
    {
        var user = GetByEmail(email);

        if (user is null)
        {
            return null;
        }

        return user.PasswordHash == PasswordHasher.HashPassword(password, user.PasswordSalt) ? user : null;
    }

    public UserAccount Register(RegisterViewModel registration)
    {
        if (GetByEmail(registration.Email) is not null)
        {
            throw new InvalidOperationException("An account already exists for this email.");
        }

        var salt = PasswordHasher.CreateSalt();
        var user = new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            FullName = registration.FullName,
            Email = registration.Email.Trim().ToLowerInvariant(),
            Phone = registration.Phone,
            DefaultAddress = registration.DefaultAddress,
            Role = UserRoles.Customer,
            PasswordSalt = salt,
            PasswordHash = PasswordHasher.HashPassword(registration.Password, salt)
        };

        return _dataStore.AddUser(user);
    }

    public UserAccount? GetById(string id)
    {
        return _dataStore.GetSnapshot().Users.FirstOrDefault(user => user.Id == id);
    }

    public UserAccount? GetByEmail(string email)
    {
        return _dataStore.GetSnapshot().Users.FirstOrDefault(user => user.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<UserAccount> GetUsers()
    {
        return _dataStore.GetSnapshot().Users;
    }

    public UserAccount? UpdateProfile(string id, ProfileUpdateViewModel profile)
    {
        return _dataStore.UpdateUserProfile(
            id,
            profile.FullName.Trim(),
            string.IsNullOrWhiteSpace(profile.Phone) ? null : profile.Phone.Trim(),
            string.IsNullOrWhiteSpace(profile.DefaultAddress) ? null : profile.DefaultAddress.Trim());
    }

    public bool ChangePassword(string id, PasswordChangeViewModel password)
    {
        var user = GetById(id);

        if (user is null ||
            user.PasswordHash != PasswordHasher.HashPassword(password.CurrentPassword, user.PasswordSalt))
        {
            return false;
        }

        var salt = PasswordHasher.CreateSalt();
        return _dataStore.UpdateUserPassword(id, PasswordHasher.HashPassword(password.NewPassword, salt), salt);
    }
}
