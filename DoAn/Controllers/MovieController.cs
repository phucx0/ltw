using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Movies()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
