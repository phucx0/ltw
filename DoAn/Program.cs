using DoAn.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ModelContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

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

// Kích hoạt middleware Authentication
// - Middleware này sẽ kiểm tra request có cookie/token hợp lệ hay không
// - Phải đặt trước UseAuthorization() để hệ thống xác định danh tính user trước khi kiểm tra quyền
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
