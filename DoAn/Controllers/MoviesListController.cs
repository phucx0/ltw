using DoAn.Models.Data;
using DoAn.Models.Movies;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    public class MoviesListController : Controller
    {
        private readonly ModelContext _context;

        public MoviesListController(ModelContext context)
        {
            _context = context;
        }

     
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();

            
            var featured = movies.Where(m => m.Status == "NoiBat").ToList();
            var showing = movies.Where(m => m.Status == "DangChieu").ToList();
            var upcoming = movies.Where(m => m.Status == "SapChieu").ToList();

            ViewBag.Featured = featured;
            ViewBag.Showing = showing;
            ViewBag.Upcoming = upcoming;

            return View();
        }
    }
}