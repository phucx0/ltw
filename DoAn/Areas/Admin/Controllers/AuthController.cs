using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        private readonly PasswordHasher<Models.Accounts.User> _passwordHasher = new PasswordHasher<Models.Accounts.User>();

        public AuthController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public string HashPassword(Models.Accounts.User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(Models.Accounts.User user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!Helpers.Validator.IsValidGmail(email))
            {
                ViewBag.Error = "Email không đúng định dạng";
                return View();
            }
            //if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            //{
            //    ViewBag.Error = "Mật khẩu phải có ít nhất 8 ký tự";
            //    return View();
            //}
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var user = db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại";
                return View();
            }

            bool result = VerifyPassword(user, user.PasswordHash, password);

            if (!result)
            {
                ViewBag.Error = "Sai mật khẩu";
                return View();
            }

            //  Lấy danh sách PERMISSION theo ROLE
            var permissionService = new PermissionService(_dbFactory);
            var permissions = permissionService.GetPermissionsByRoleId(user.RoleId.Value);


            // Tạo Claims
            var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.RoleName.ToLower()),
            new Claim(ClaimTypes.Email, user.Email),

            //  Nhét toàn bộ permission vào 1 claim (dạng string)
            new Claim("permissions", string.Join(",", permissions))
             };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Tạo cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(12)
                }
            );

            return RedirectToAction("Index", "Dashboard");
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logout]: {ex.Message}");
            }
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
