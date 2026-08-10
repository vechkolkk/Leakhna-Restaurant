using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".aspnet-data-protection-keys")))
    .SetApplicationName("LeakhnasRestaurant");
builder.Services.Configure<RestaurantApp.Options.PersistenceOptions>(
    builder.Configuration.GetSection(RestaurantApp.Options.PersistenceOptions.SectionName));
builder.Services.Configure<RestaurantApp.Options.MongoDbOptions>(
    builder.Configuration.GetSection(RestaurantApp.Options.MongoDbOptions.SectionName));

var persistenceProvider = builder.Configuration
    .GetSection(RestaurantApp.Options.PersistenceOptions.SectionName)
    .GetValue<string>(nameof(RestaurantApp.Options.PersistenceOptions.Provider)) ?? "Json";

if (persistenceProvider.Equals("Json", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<RestaurantApp.Services.IRestaurantDataStore, RestaurantApp.Services.JsonRestaurantDataStore>();
    builder.Services.AddSingleton<RestaurantApp.Services.IMenuService, RestaurantApp.Services.PersistentMenuService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IOrderService, RestaurantApp.Services.PersistentOrderService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IUserService, RestaurantApp.Services.PersistentUserService>();
}
else if (persistenceProvider.Equals("MongoDb", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<RestaurantApp.Services.IRestaurantDataStore, RestaurantApp.Services.MongoRestaurantDataStore>();
    builder.Services.AddSingleton<RestaurantApp.Services.IMenuService, RestaurantApp.Services.PersistentMenuService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IOrderService, RestaurantApp.Services.PersistentOrderService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IUserService, RestaurantApp.Services.PersistentUserService>();
}
else if (persistenceProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<RestaurantApp.Services.IMenuService, RestaurantApp.Services.InMemoryMenuService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IOrderService, RestaurantApp.Services.InMemoryOrderService>();
    builder.Services.AddSingleton<RestaurantApp.Services.IUserService, RestaurantApp.Services.InMemoryUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported persistence provider '{persistenceProvider}'. Use 'Json', 'MongoDb', or 'InMemory'.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
