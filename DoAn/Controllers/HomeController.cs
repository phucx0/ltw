using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class HomeController : Controller  
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error404() => View();
        public IActionResult Error() => View();
    }
}
