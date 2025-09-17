using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace DoAn.Controllers
{
    public class AuthController : Controller
    {
        private readonly ModelContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        public AuthController(ModelContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kiểm tra email tồn tại trong DB
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại!";
                return View();
            }

            // Kiểm tra mật khẩu (verify hash)
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Sai mật khẩu!";
                return View();
            }

            // Kiểm tra user có bị khóa hay không
            if (user.IsActive == false)
            {
                ViewBag.Error = "Tài khoản đã bị khóa!";
                return View();
            }

            // Tạo Claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
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
                    IsPersistent = true, // Lưu đăng nhập qua nhiều session
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) // Cookie hết hạn sau 7 ngày
                }
            );

            TempData["Message"] = "Đăng nhập thành công!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public IActionResult Register(string fullname, string phone, string email, DateTime birthdate, string password, string confirmPassword)
        {
            // Kiểm tra trùng email
            if (_context.Users.Count(u => u.Email == email) > 0)
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View();
            }
            if (_context.Users.Count(u => u.Phone == phone) > 0)
            {
                ViewBag.Error = "Số điện thoại đã tồn tại!";
                return View();
            }

            // Kiểm tra xác nhận mật khẩu
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Tạo user mới
            var user = new User
            {
                FullName = fullname,
                Phone = phone,
                Email = email,
                Birthday = birthdate,
                CreatedAt = DateTime.Now,
                Role = "user",
                IsActive = true
            };

            // Hash mật khẩu
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Lưu DB
            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Message"] = "Đăng ký thành công!";
            return RedirectToAction("Login", "Auth");
        }


        // GET: /Auth/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserEmail"); // xoá cookie
            return RedirectToAction("Login");
        }
    }
}
