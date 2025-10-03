    using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn.Models.Movies;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;

        public HomeController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var trendingMovies = _context.Movies
                             .OrderByDescending(m => m.ImdbRating)
                             .Take(5)
                             .ToList();


            return View(trendingMovies); // Gửi qua View
        }

        public IActionResult Error404() => View();
        public IActionResult Error() => View();
    }
}
