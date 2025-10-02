using DoAn.Models.Movies;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Controllers
{
    public class MovieController : Controller
    {
        private List<Movie> GetAllMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Title = "Thanh gươm diệt quỷ",
                    Directors = new List<Director>
                    {
                        new Director { DirectorName = "Koyoharu Gotouge" },
                        new Director { DirectorName = "Haruo Sotozaki" }
                    },
                    PosterUrl= "https://iguov8nhvyobj.vcdn.cloud/media/catalog/product/cache/1/thumbnail/240x388/c88460ec71d04fa96e628a21494d2fd3/p/o/poster_dm.jpg",
                },
                new Movie
                {
                    Title = "One Piece Film: Red",
                    Directors = new List<Director>
                    {
                        new Director { DirectorName = "Eiichiro Oda" }
                    },
                    PosterUrl= "https://static.nutscdn.com/vimg/300-0/3032221de3bc86bcfbc979635b53d047.jpg",
                },
                new Movie
                {
                    Title = "Jujutsu Kaisen 0",
                    Directors = new List<Director>
                    {
                        new Director { DirectorName = "Gege Akutami" }
                    },
                    PosterUrl= "https://static.nutscdn.com/vimg/300-0/09e4c57b152db1ecdb639eadef6b9356.jpg",
                },
                new Movie
                {
                    Title = "Your Name",
                    Directors = new List<Director>
                    {
                        new Director { DirectorName = "Makoto Shinkai" }
                    },
                    PosterUrl= "https://static.nutscdn.com/vimg/300-0/1cb31180dc6e0d527a7faef6918b6bb3.jpg",
                },
                new Movie
                {
                    Title = "Spirited Away",
                    Directors = new List<Director>
                    {
                        new Director { DirectorName = "Hayao Miyazaki" }
                    },
                    PosterUrl= "https://static.nutscdn.com/vimg/300-0/f416e981c5594516dcdedede5c359895.jpg",
                }
            };
        }

        public IActionResult Movies(string? query)
        {
            var allMovies = GetAllMovies();

            // Nếu có query → lọc danh sách
            if (!string.IsNullOrEmpty(query))
            {
                allMovies = allMovies
                    .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            MoviesPageViewModel model = new MoviesPageViewModel
            {
                TredingMovies = allMovies,
                NowShowingMovies = allMovies
            };

            return View(model);
        }

        public IActionResult Details(string title)
        {
            var movie = GetAllMovies()
                .FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (movie == null) return NotFound();

            return View(movie);
        }
    }
}
