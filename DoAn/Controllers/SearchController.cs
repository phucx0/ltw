using DoAn.Models.Data;
using DoAn.Models.Movies;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class SearchController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public SearchController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // Trang search chính (hiện kết quả theo query)
        public IActionResult Index(string query)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var result = db.Movies
                .Where(m => m.Title.ToLower().Contains(query))
                .Take(10)
                .ToList();

            return View(result);
        }

    }
}
