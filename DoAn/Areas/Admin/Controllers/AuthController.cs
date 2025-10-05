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
        private readonly ModelContext _context;
        private readonly PasswordHasher<Models.Accounts.User> _passwordHasher = new PasswordHasher<Models.Accounts.User>();

        public AuthController(ModelContext context)
        {
            _context = context;
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

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Role.RoleName == "admin");
            //&& u.PasswordHash == HashPassword(password)
            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại";
                return View();
            }

            string hashed = HashPassword(user, password);
            var result = VerifyPassword(user, user.PasswordHash, password);

            if (result)
            {
                // Tạo Claims
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName.ToString().ToLower()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Đăng nhập và tạo cookie authentication
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

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác";
            return View();
        }
    }
}
