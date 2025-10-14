using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsPaperWebApp.Data;
using NewsPaperWebApp.Services; // ✅ Add this to use NewsService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // ✅ Required for NewsService API calls

// 🔹 Register NewsService for Dependency Injection
builder.Services.AddScoped<NewsService>(); // ✅ FIX: Register NewsService

var app = builder.Build();

// 🔹 Seed the default admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDefaultUser(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Ensure HTTPS redirection
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Default route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// 🔹 Method to seed default user
async Task SeedDefaultUser(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminRole = "Admin";
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";

    // Ensure the Admin Role Exists
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Check if an admin user already exists
    var existingUsers = await userManager.Users
        .Where(u => u.NormalizedEmail == adminEmail.ToUpper())
        .ToListAsync();

    if (existingUsers.Count == 0) // ✅ Prevents duplicate creation
    {
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("✅ Admin user created successfully.");
        }
        else
        {
            Console.WriteLine("⚠️ Failed to create admin user.");
        }
    }
    else
    {
        Console.WriteLine("⚠️ Admin user already exists, skipping creation.");
    }
}
