using F1Tickets.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using F1Tickets.Areas.Identity.Data;
using F1Tickets.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//
builder.Services.AddHttpClient(); // Register HttpClient
builder.Services.AddScoped<IEmailService, EmailService>();  //Register EmailService 
builder.Services.AddHttpClient<ITrackInfoService, TrackInfoService>(); //Register TrsckInfoService
builder.Services.AddHttpClient<F1ResultsService>();
//

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("AuthDbContextConnection")));


builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Adding Roles
using(var scope = app.Services.CreateScope())
{
    var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var Roles = new[] { "Admin", "User" };
    foreach (var Role in Roles)
    {
        if (!await RoleManager.RoleExistsAsync(Role))
            await RoleManager.CreateAsync(new IdentityRole(Role));
    }

}
//Adding Admin acount
using(var scope = app.Services.CreateScope())
{
    var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    string email = "Admin@admin.com";
    string password = "Aa_123456789";

    if (await UserManager.FindByEmailAsync(email) == null)
    {
        var User = new AppUser();
        User.UserName = email;
        User.Email = email;

        await UserManager.CreateAsync(User, password);

        await UserManager.AddToRoleAsync(User, "Admin");
    }

}

app.Run();
