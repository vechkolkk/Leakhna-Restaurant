using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IUserService
{
    UserAccount? Authenticate(string email, string password);

    UserAccount Register(RegisterViewModel registration);

    UserAccount? GetById(string id);

    UserAccount? GetByEmail(string email);

    IReadOnlyList<UserAccount> GetUsers();

    UserAccount? UpdateProfile(string id, ProfileUpdateViewModel profile);

    bool AddFavorite(string id, string menuItemId);

    bool RemoveFavorite(string id, string menuItemId);

    bool ChangePassword(string id, PasswordChangeViewModel password);
}
