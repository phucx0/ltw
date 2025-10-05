using DoAn.Models.Data;
using DoAn.Models.Movies;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class SearchController : Controller
    {
        private ModelContext _context;
        public SearchController(ModelContext context)
        {
            _context = context;
        }

        // Trang search chính (hiện kết quả theo query)
        public IActionResult Index(string query)
        {
            var result = _context.Movies
                .Where(m => m.Title.ToLower().Contains(query))
                .Take(10)
                .ToList();

            return View(result);
        }

    }
}
