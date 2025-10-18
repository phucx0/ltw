using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class ShowtimesController : Controller
    {
        private ModelContext _context;
        public ShowtimesController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Take(10)
                .ToList();
            return View(showtimes);
        }
    }
}
