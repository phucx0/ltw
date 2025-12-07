using DoAn.Areas.Booking.Services;
using DoAn.Models.Data;
using DoAn.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<HoldCleanupService>();
builder.Services.AddHttpClient<PaymentService>();
builder.Services.AddScoped<BookingService>();

// Giảm log EF Core
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Connection", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);

// Cấu hình Authentication sử dụng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Đường dẫn chuyển hướng khi chưa đăng nhập
        options.LoginPath = "/Auth/Login";
        // Đường dẫn chuyển hướng khi logout
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Khi bị chặn quyền
        options.ExpireTimeSpan = TimeSpan.FromDays(7);   // Thời gian sống của cookie
        options.SlidingExpiration = true;        // Tự gia hạn khi user hoạt động
    });

var app = builder.Build();

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

// API route — PHẢI nằm sau UseRouting và trước MapDefaultControllerRoute
//app.MapControllers();

// Route MVC thường
//app.MapDefaultControllerRoute();

// Route Areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
    defaults: new { action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<PaymentHub>("/paymentHub");

app.Run();
