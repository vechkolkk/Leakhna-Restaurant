using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class AccountController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IMenuService _menuService;
    private readonly IUserService _userService;

    public AccountController(IUserService userService, IOrderService orderService, IMenuService menuService)
    {
        _userService = userService;
        _orderService = orderService;
        _menuService = menuService;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var user = _userService.Authenticate(login.Email, login.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "We could not sign you in with that email and password.");
            return View(login);
        }

        await SignInUser(user, login.RememberMe);
        return RedirectToLocal(login.ReturnUrl);
    }

    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registration)
    {
        if (!ModelState.IsValid)
        {
            return View(registration);
        }

        try
        {
            var user = _userService.Register(registration);
            await SignInUser(user, isPersistent: false);
            return RedirectToAction(nameof(Profile));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(registration.Email), ex.Message);
            return View(registration);
        }
    }

    [Authorize]
    public IActionResult Profile()
    {
        var user = GetCurrentUser();

        if (user is null)
        {
            return Challenge();
        }

        return View(new ProfileViewModel
        {
            User = user,
            Profile = ProfileUpdateViewModelFromUser(user),
            Orders = _orderService.GetOrdersForCustomer(user.Id, user.Email),
            FavoriteMenuItems = GetFavoriteMenuItems(user)
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileUpdateViewModel profile)
    {
        var user = GetCurrentUser();

        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Profile), new ProfileViewModel
            {
                User = user,
                Profile = profile,
                Orders = _orderService.GetOrdersForCustomer(user.Id, user.Email),
                FavoriteMenuItems = GetFavoriteMenuItems(user)
            });
        }

        var updatedUser = _userService.UpdateProfile(user.Id, profile);

        if (updatedUser is null)
        {
            return NotFound();
        }

        await SignInUser(updatedUser, isPersistent: false);
        TempData["ProfileMessage"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(PasswordChangeViewModel password)
    {
        var user = GetCurrentUser();

        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Profile), BuildProfileViewModel(user, password));
        }

        if (!_userService.ChangePassword(user.Id, password))
        {
            ModelState.AddModelError("Password.CurrentPassword", "That does not match your current password.");
            return View(nameof(Profile), BuildProfileViewModel(user, password));
        }

        TempData["ProfileMessage"] = "Password updated.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder(string id)
    {
        var user = GetCurrentUser();

        if (user is null)
        {
            return Challenge();
        }

        var order = _orderService.GetOrder(id);

        if (order is null)
        {
            return NotFound();
        }

        if (!IsCustomerOrder(order, user))
        {
            return Forbid();
        }

        if (!OrderStatusOptions.CanCustomerCancel(order.Status))
        {
            TempData["ProfileMessage"] = $"Receipt {order.Id} can no longer be cancelled.";
            return RedirectToAction(nameof(Profile));
        }

        _orderService.UpdateOrderStatus(order.Id, "Cancelled");
        TempData["ProfileMessage"] = $"Receipt {order.Id} was cancelled.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFavorite(string id)
    {
        var user = GetCurrentUser();

        if (user is null)
        {
            return Challenge();
        }

        _userService.RemoveFavorite(user.Id, id);
        TempData["ProfileMessage"] = "Favorite removed.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private UserAccount? GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId) ? null : _userService.GetById(userId);
    }

    private static ProfileUpdateViewModel ProfileUpdateViewModelFromUser(UserAccount user)
    {
        return new ProfileUpdateViewModel
        {
            FullName = user.FullName,
            Phone = user.Phone,
            DefaultAddress = user.DefaultAddress
        };
    }

    private ProfileViewModel BuildProfileViewModel(UserAccount user, PasswordChangeViewModel? password = null)
    {
        return new ProfileViewModel
        {
            User = user,
            Profile = ProfileUpdateViewModelFromUser(user),
            Password = password ?? new PasswordChangeViewModel(),
            Orders = _orderService.GetOrdersForCustomer(user.Id, user.Email),
            FavoriteMenuItems = GetFavoriteMenuItems(user)
        };
    }

    private IReadOnlyList<MenuItem> GetFavoriteMenuItems(UserAccount user)
    {
        var favorites = user.FavoriteMenuItemIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return _menuService.GetMenuItems()
            .Where(item => favorites.Contains(item.Id))
            .OrderBy(item => item.Name)
            .ToList();
    }

    private static bool IsCustomerOrder(Order order, UserAccount user)
    {
        return order.CustomerId == user.Id ||
            order.Email?.Equals(user.Email, StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task SignInUser(UserAccount user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = isPersistent });
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
