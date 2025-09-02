using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
