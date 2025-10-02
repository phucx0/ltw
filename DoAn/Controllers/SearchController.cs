using DoAn.Models.Movies;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class SearchController : Controller
    {
        private static List<Movie> _movies = new List<Movie>
        {
            new Movie { MovieId = 1, Title="Thanh gươm diệt quỷ", 
                PosterUrl="https://iguov8nhvyobj.vcdn.cloud/media/catalog/product/cache/1/thumbnail/240x388/c88460ec71d04fa96e628a21494d2fd3/p/o/poster_dm.jpg" },
            
            new Movie { MovieId = 2, Title="One Piece Film: Red", 
                PosterUrl="https://static.nutscdn.com/vimg/300-0/3032221de3bc86bcfbc979635b53d047.jpg" },
            
            new Movie { MovieId = 3, Title="Jujutsu Kaisen 0", 
                PosterUrl="https://static.nutscdn.com/vimg/300-0/09e4c57b152db1ecdb639eadef6b9356.jpg" },
            
            new Movie { MovieId = 4, Title="Your Name", 
                PosterUrl="https://static.nutscdn.com/vimg/300-0/1cb31180dc6e0d527a7faef6918b6bb3.jpg" },
            
            new Movie { MovieId = 5, Title="Spirited Away", 
                PosterUrl="https://static.nutscdn.com/vimg/300-0/f416e981c5594516dcdedede5c359895.jpg" }
        };

        // Trang search chính (hiện kết quả theo query)
        public IActionResult Index(string query)
        {
            var result = string.IsNullOrEmpty(query)
                ? _movies
                : _movies
                    .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return View(result);
        }

        // API suggest (dùng AJAX/autocomplete)
        public IActionResult Suggest(string q)
        {
            var result = _movies
                .Where(m => m.Title.Contains(q ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Select(m => new 
                { 
                    id = m.MovieId, 
                    title = m.Title, 
                    posterUrl = m.PosterUrl 
                })
                .ToList();

            return Json(result);
        }
    }
}
