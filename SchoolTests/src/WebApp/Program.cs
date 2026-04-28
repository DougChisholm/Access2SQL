using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? "Data Source=schooltests.db";

builder.Services.AddDbContext<SchoolTestsDbContext>(options =>
    options.UseSqlite(connStr));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit            = true;
    options.Password.RequireLowercase        = true;
    options.Password.RequireUppercase        = true;
    options.Password.RequireNonAlphanumeric  = false;
    options.Password.RequiredLength          = 8;
    options.SignIn.RequireConfirmedAccount   = false;
})
.AddEntityFrameworkStores<SchoolTestsDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name      = "SchoolTests.Auth";
});

// ── App pipeline ──────────────────────────────────────────────────────────────
var app = builder.Build();

// Auto-create database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db          = scope.ServiceProvider.GetRequiredService<SchoolTestsDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    db.Database.EnsureCreated();
    await DbInitializer.SeedAsync(db, userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
