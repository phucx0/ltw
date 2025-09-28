using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
