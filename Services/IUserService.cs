using RestaurantApp.Models;

namespace RestaurantApp.Services;

public interface IUserService
{
    UserAccount? Authenticate(string email, string password);

    UserAccount Register(RegisterViewModel registration);

    UserAccount? GetById(string id);

    UserAccount? GetByEmail(string email);

    UserAccount? UpdateProfile(string id, ProfileUpdateViewModel profile);
}
