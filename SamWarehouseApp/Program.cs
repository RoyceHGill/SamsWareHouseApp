using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamWarehouseApp.Areas.Identity.Data;
using SamWarehouseApp.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SamWarehouseAppContextConnection") ?? throw new InvalidOperationException("Connection string 'SamWarehouseAppContextConnection' not found.");

builder.Services.AddDbContext<SamWarehouseAppContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SamWarehouseAppContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ShoppingLists}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
