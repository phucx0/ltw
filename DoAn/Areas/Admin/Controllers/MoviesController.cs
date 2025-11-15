using DoAn.Models.Data;
using DoAn.Models.Movies;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class MoviesController : Controller
    {
        private ModelContext _context;
        public MoviesController (ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.Movies.Take(10).ToList();
            return View(list);
        }

        public IActionResult Create()
        {
            var vm = new MovieViewModel
            {
                AllActors = _context.Actors.ToList(),
                AllDirectors = _context.Directors.ToList(),
                AllRatings = _context.AgeRatings.ToList(),
                AllGenres = new List<string> { "Action", "Comedy", "Crime", "Drama", "Fantasy", "Horror", "Sci-Fi" }
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllActors = _context.Actors.ToList();
                vm.AllDirectors = _context.Directors.ToList();
                vm.AllRatings = _context.AgeRatings.ToList();
                vm.AllGenres = new List<string> { "Action", "Comedy", "Crime", "Drama", "Fantasy", "Horror", "Sci-Fi" };
                return View(vm);
            }

            var movie = new Movie
            {
                Title = vm.Title,
                Description = vm.Description,
                Duration = vm.Duration,
                Genre = string.Join(",", vm.SelectedGenres ?? new List<string>()),
                RatingId = vm.RatingId,
                ReleaseDate = vm.ReleaseDate,
                PosterUrl = vm.PosterUrl,
                CoverUrl = vm.CoverUrl,
                TrailerUrl = vm.TrailerUrl,
                Status = vm.Status,
                ImdbRating = vm.ImdbRating
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            // Add Actors
            foreach (var actorId in vm.SelectedActorIds)
                _context.MovieActors.Add(new MovieActor { MovieId = movie.MovieId, ActorId = actorId });

            // Add Directors
            foreach (var directorId in vm.SelectedDirectorIds)
                _context.MovieDirectors.Add(new MovieDirector { MovieId = movie.MovieId, DirectorId = directorId });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.AgeRating)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieDirectors)
                    .ThenInclude(md => md.Director)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            return View(movie);
        }

        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieDirectors)
                .FirstOrDefault(m => m.MovieId == id);

            if (movie == null) return NotFound();

            var vm = new MovieViewModel
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                Genre = movie.Genre,
                RatingId = movie.RatingId,
                ReleaseDate = movie.ReleaseDate,
                PosterUrl = movie.PosterUrl,
                CoverUrl = movie.CoverUrl,
                TrailerUrl = movie.TrailerUrl,
                Status = movie.Status,
                ImdbRating = movie.ImdbRating,
                SelectedGenres = movie.Genre?.Split(",")?.ToList() ?? new List<string>(),
                SelectedActorIds = movie.MovieActors.Select(x => x.ActorId).ToList(),
                SelectedDirectorIds = movie.MovieDirectors.Select(x => x.DirectorId).ToList(),

                AllActors = _context.Actors.ToList(),
                AllDirectors = _context.Directors.ToList(),
                AllRatings = _context.AgeRatings.ToList()
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(MovieViewModel vm)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieDirectors)
                .FirstOrDefaultAsync(m => m.MovieId == vm.MovieId);

            if (movie == null) return NotFound();

            movie.Title = vm.Title;
            movie.Description = vm.Description;
            movie.Duration = vm.Duration;
            movie.Genre = string.Join(",", vm.SelectedGenres);
            movie.RatingId = vm.RatingId;
            movie.ReleaseDate = vm.ReleaseDate;
            movie.PosterUrl = vm.PosterUrl;
            movie.CoverUrl = vm.CoverUrl;
            movie.TrailerUrl = vm.TrailerUrl;
            movie.Status = vm.Status;
            movie.ImdbRating = vm.ImdbRating;

            // Update MovieActors
            _context.MovieActors.RemoveRange(movie.MovieActors);
            foreach (var id in vm.SelectedActorIds)
                _context.MovieActors.Add(new MovieActor { MovieId = movie.MovieId, ActorId = id });

            // Update MovieDirectors
            _context.MovieDirectors.RemoveRange(movie.MovieDirectors);
            foreach (var id in vm.SelectedDirectorIds)
                _context.MovieDirectors.Add(new MovieDirector { MovieId = movie.MovieId, DirectorId = id });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
