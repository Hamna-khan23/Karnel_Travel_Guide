using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Cookie Authentication
// ONE LOGIN PAGE FOR BOTH USER AND ADMIN
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "KarnelTravelAuthCookie";

        options.LoginPath = "/Accounts/Login";

        options.AccessDeniedPath = "/Accounts/Login";

        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        options.ReturnUrlParameter = "returnUrl";
    });

var app = builder.Build();


// =========================
// CREATE DEFAULT ADMIN
// =========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var adminEmail = "admin@karnel.com";

    var adminExists = await context.Users
        .AnyAsync(u => u.Email == adminEmail);

    if (!adminExists)
    {
        var admin = new User
        {
            Name = "Administrator",
            Email = adminEmail,
            PasswordHash = "admin123",
            Role = "Admin",
            Phone = "03000000000",
            CreatedDate = DateTime.Now
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

// Admin Area Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Normal Controller Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
)
.WithStaticAssets();

app.Run();