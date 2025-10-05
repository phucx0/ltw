using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class MovieController : Controller
    {
        private ModelContext _context;
        public MovieController (ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.Movies.Take(10).ToList();
            return View(list);
        }
    }
}
