using DoAn.Models.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile(User user)
        {
            return View(user);
        }
    }
}
