using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DoAn.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email.Contains("@gmail.com"))
            {
                if (email == "admin@gmail.com" && password == "123")
                {
                    TempData["Message"] = "Đăng nhập thành công!";
                    Response.Cookies.Append("UserEmail", email, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true
                    });
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View();
            }
            ViewBag.Error = "Không đúng định dạng email";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        // POST: /Auth/Register
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            List<string> Emails = ["admin@gmail.com", "admin1@gmail.com", "admin2@gmail.com"];
            if (Emails.Any(e => e == email))
            {
                ViewBag.Error = "Tài khoản đã tồn tại!";
                return View();
            }
            //TempData["Message"] = "Đăng ký thành công!";
            return RedirectToAction("Index", "Home");
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
