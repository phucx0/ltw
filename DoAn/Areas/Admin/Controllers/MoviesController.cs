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
        private readonly IDbContextFactory _dbFactory;
        public MoviesController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var list = db.Movies.Take(10).ToList();
            return View(list);
        }

        public IActionResult Create()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var vm = new MovieViewModel
            {
                AllActors = db.Actors.ToList(),
                AllDirectors = db.Directors.ToList(),
                AllRatings = db.AgeRatings.ToList(),
                AllGenres = new List<string> { "Action", "Comedy", "Crime", "Drama", "Fantasy", "Horror", "Sci-Fi" }
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel vm)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            if (!ModelState.IsValid)
            {
                vm.AllActors = db.Actors.ToList();
                vm.AllDirectors = db.Directors.ToList();
                vm.AllRatings = db.AgeRatings.ToList();
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

            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            // Add Actors
            foreach (var actorId in vm.SelectedActorIds)
                db.MovieActors.Add(new MovieActor { MovieId = movie.MovieId, ActorId = actorId });

            // Add Directors
            foreach (var directorId in vm.SelectedDirectorIds)
                db.MovieDirectors.Add(new MovieDirector { MovieId = movie.MovieId, DirectorId = directorId });

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Details(int id)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var movie = await db.Movies
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
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var movie = db.Movies
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

                AllActors = db.Actors.ToList(),
                AllDirectors = db.Directors.ToList(),
                AllRatings = db.AgeRatings.ToList()
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(MovieViewModel vm)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var movie = await db.Movies
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
            db.MovieActors.RemoveRange(movie.MovieActors);
            foreach (var id in vm.SelectedActorIds)
                db.MovieActors.Add(new MovieActor { MovieId = movie.MovieId, ActorId = id });

            // Update MovieDirectors
            db.MovieDirectors.RemoveRange(movie.MovieDirectors);
            foreach (var id in vm.SelectedDirectorIds)
                db.MovieDirectors.Add(new MovieDirector { MovieId = movie.MovieId, DirectorId = id });

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var movie = await db.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var movie = await db.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            // Delete MovieActors
            var movieActors = db.MovieActors.Where(ma => ma.MovieId == id);
            db.MovieActors.RemoveRange(movieActors);

            // Delete MovieDirectors
            var movieDirectors = db.MovieDirectors.Where(md => md.MovieId == id);
            db.MovieDirectors.RemoveRange(movieDirectors);

            // Delete Movie
            db.Movies.Remove(movie);

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
