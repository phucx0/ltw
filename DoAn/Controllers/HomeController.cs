    using DoAn.Models.Data;
using DoAn.Models.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public HomeController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var trendingMovies = db.Movies
                             .OrderByDescending(m => m.ImdbRating)
                             .Take(5)
                             .ToList();

            return View(trendingMovies);
        }

        public IActionResult Error404() => View();
        public IActionResult Error() => View();
    }
}
