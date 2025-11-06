using DoAn.Models.Data;
using DoAn.Models.Movies;
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

        public IActionResult Create() {
            return View();
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

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            return RedirectToAction("Index", "Movies");
            //var movie = await _context.Movies.FindAsync(id);
            //return View(movie);
        }
    }
}
