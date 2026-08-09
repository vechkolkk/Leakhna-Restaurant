using Microsoft.AspNetCore.DataProtection;

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
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".aspnet-data-protection-keys")))
    .SetApplicationName("LeakhnasRestaurant");
builder.Services.Configure<RestaurantApp.Options.MongoDbOptions>(
    builder.Configuration.GetSection(RestaurantApp.Options.MongoDbOptions.SectionName));
builder.Services.AddSingleton<RestaurantApp.Services.IMenuService, RestaurantApp.Services.InMemoryMenuService>();
builder.Services.AddSingleton<RestaurantApp.Services.IOrderService, RestaurantApp.Services.InMemoryOrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
