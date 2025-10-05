using DoAn.Models.Data;
using DoAn.Models.Movies;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DoAn.Controllers
{
    public class MovieController : Controller
    {
        private readonly ModelContext _context;

        public MovieController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult Movies()
        {
            List<Movie> list = _context.Movies.ToList();
            List<Movie> treding_movies = list.Where(m => m.Status.ToLower() == "now showing").ToList();
            List<Movie> now_showing_movies = list.Where(m => m.Status.ToLower() == "now showing").ToList();
            List<Movie> coming_soon_movies = list.Where(m => m.Status.ToLower() == "coming soon").ToList();

            MoviesPageViewModel model = new MoviesPageViewModel
            {
                TredingMovies = treding_movies,
                NowShowingMovies = now_showing_movies,
                ComingSoonMovies = coming_soon_movies
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return RedirectToAction("Error404", "Home");
            }
            var movie = await _context.Movies
                .Include(m => m.AgeRating)
                //.Include(m => m.Directors)
                //.Include(m => m.Actors)
                .Include(m => m.MovieDirectors)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            return View(movie);
        }
    }
}
