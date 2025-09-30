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
            // Lấy danh sách phim nổi bật (status = "Hot")
            var trendingMovies = _context.Movies
                                         .Where(m => m.Status == 1)
                                         .OrderByDescending(m => m.ReleaseDate)
                                         .Take(10)
                                         .ToList();

            return View(trendingMovies); // Gửi qua View
        }
    }
}
