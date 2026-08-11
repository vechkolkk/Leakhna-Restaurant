using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
    private readonly IReviewService _reviewService;
    private readonly IUserService _userService;

    public MenuController(
        IMenuService menuService,
        IOrderService orderService,
        IReviewService reviewService,
        IUserService userService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _reviewService = reviewService;
        _userService = userService;
    }

    public IActionResult Index(
        string? category,
        string? dietaryTag,
        string? avoidAllergen,
        string? search,
        bool availableOnly = true)
    {
        var menuItems = _menuService.GetMenuItems().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            menuItems = menuItems
                .Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(dietaryTag))
        {
            menuItems = menuItems
                .Where(item => item.DietaryTags.Any(tag => tag.Equals(dietaryTag, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(avoidAllergen))
        {
            menuItems = menuItems
                .Where(item => !item.Allergens.Any(allergen => allergen.Equals(avoidAllergen, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            menuItems = menuItems.Where(item =>
                item.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Ingredients.Any(ingredient => ingredient.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                item.DietaryTags.Any(tag => tag.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                item.Allergens.Any(allergen => allergen.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (availableOnly)
        {
            menuItems = menuItems.Where(item => item.IsAvailable);
        }

        return View(new MenuIndexViewModel
        {
            Items = menuItems.OrderBy(item => item.Category).ThenBy(item => item.Name).ToList(),
            ReviewSummaries = _reviewService.GetReviewSummaries(),
            Categories = _menuService.GetCategories(),
            DietaryTags = _menuService.GetDietaryTags(),
            Allergens = _menuService.GetAllergens(),
            Category = category,
            DietaryTag = dietaryTag,
            AvoidAllergen = avoidAllergen,
            Search = search,
            AvailableOnly = availableOnly
        });
    }

    public IActionResult Details(string id)
    {
        var menuItem = _menuService.GetMenuItem(id);

        if (menuItem is null)
        {
            return NotFound();
        }

        return View(BuildDetailsViewModel(menuItem, new ReviewFormViewModel
        {
            ReviewerName = User.Identity?.IsAuthenticated == true ? User.Identity.Name ?? string.Empty : string.Empty
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddReview(string id, [Bind(Prefix = "ReviewForm")] ReviewFormViewModel review)
    {
        var menuItem = _menuService.GetMenuItem(id);

        if (menuItem is null)
        {
            return NotFound();
        }

        var currentUser = GetCurrentUser();

        if (!CanReviewMenuItem(menuItem.Id, currentUser))
        {
            TempData["ReviewMessage"] = "Only signed-in customers who bought this dish can leave a review.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!ModelState.IsValid)
        {
            return View("Details", BuildDetailsViewModel(menuItem, review));
        }

        _reviewService.AddReview(id, review, currentUser);
        TempData["ReviewMessage"] = "Thanks for reviewing this dish.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private MenuDetailsViewModel BuildDetailsViewModel(MenuItem menuItem, ReviewFormViewModel reviewForm)
    {
        var reviews = _reviewService.GetReviewsForMenuItem(menuItem.Id);
        var summary = _reviewService.GetReviewSummaries().GetValueOrDefault(menuItem.Id) ??
            new ReviewSummary { MenuItemId = menuItem.Id };
        var currentUser = GetCurrentUser();
        var canReview = CanReviewMenuItem(menuItem.Id, currentUser);

        return new MenuDetailsViewModel
        {
            Item = menuItem,
            Reviews = reviews,
            ReviewSummary = summary,
            ReviewForm = reviewForm,
            CanReview = canReview,
            ReviewGateMessage = GetReviewGateMessage(currentUser, canReview)
        };
    }

    private UserAccount? GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(userId) ? null : _userService.GetById(userId);
    }

    private bool CanReviewMenuItem(string menuItemId, UserAccount? user)
    {
        if (user is null)
        {
            return false;
        }

        return _orderService.GetOrdersForCustomer(user.Id, user.Email)
            .Any(order =>
                order.Status != "Cancelled" &&
                order.Lines.Any(line => line.MenuItem.Id == menuItemId));
    }

    private static string GetReviewGateMessage(UserAccount? user, bool canReview)
    {
        if (canReview)
        {
            return string.Empty;
        }

        return user is null
            ? "Sign in with the account used for checkout to review dishes you bought."
            : "You can review this dish after buying it.";
    }
}
