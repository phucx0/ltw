using DoAn.Models.Booking;
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
        private readonly IDbContextFactory _dbFactory;

        public MovieController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Movies()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            List<Movie> treding_movies = db.Movies.Where(m => m.Status.ToLower() == "now showing").ToList();
            List<Movie> now_showing_movies = db.Movies.Where(m => m.Status.ToLower() == "now showing").ToList();
            List<Movie> coming_soon_movies = db.Movies.Where(m => m.Status.ToLower() == "coming soon").ToList();

            MoviesPageViewModel model = new MoviesPageViewModel
            {
                TredingMovies = treding_movies,
                NowShowingMovies = now_showing_movies,
                ComingSoonMovies = coming_soon_movies
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id, DateTime? date)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return RedirectToAction("Error404", "Home");
            }
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var movie = await db.Movies
                .Include(m => m.AgeRating)
                .Include(m => m.MovieDirectors)
                .Include(m => m.MovieActors)
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            // Nếu không chọn ngày, mặc định là hôm nay
            var selectedDate = date ?? DateTime.Now.Date;

            // Lấy danh sách ngày có suất chiếu 
            var showDates = movie.Showtimes
                //.Where(s => s.StartTime >= DateTime.Now)
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // Lấy suất chiếu theo ngày
            var showtimesForDate = movie.Showtimes
                .Where(s => s.StartTime.Date == selectedDate.Date)
                .ToList();

            // Group theo rạp
            var groupedShowtimes = showtimesForDate
                .GroupBy(s => s.Room.Branch)
                .ToList();

            ViewBag.ShowDates = showDates;
            ViewBag.SelectedDate = selectedDate;
            ViewBag.GroupedShowtimes = groupedShowtimes;

            return View(movie);
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimesPartial(int id, DateTime? date)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var movie = await db.Movies
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                return RedirectToAction("Error404", "Home");

            var selectedDate = date ?? DateTime.Now.Date;

            var showtimesForDate = movie.Showtimes
                .Where(s => s.StartTime.Date == selectedDate.Date)
                .ToList();

            var grouped = showtimesForDate.GroupBy(s => s.Room.Branch);
            
            return PartialView("_ShowtimesPartial", grouped);
        }


        [HttpGet]
        public async Task<IActionResult> GetSeatsByShowtime(int showtimeId)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var heldSeatIds = await db.SeatHold
                .Where(sh => sh.ShowtimeId == showtimeId && sh.ExpireAt > DateTime.Now)
                .Select(sh => sh.SeatId)
                .ToListAsync();

            var soldSeatIds = await db.BookingSeat
                .Where(bs => bs.ShowtimeId == showtimeId &&
                             bs.Booking.Status == "confirmed")
                .Select(bs => bs.SeatId)
                .ToListAsync();


            var seats = await db.Showtimes
                .Where(s => s.ShowtimeId == showtimeId)
                .SelectMany(s => s.Room.Seats)
                .Select(s => new {
                    s.SeatId,
                    s.SeatType,
                    s.SeatNumber,
                    s.SeatRow,
                    Booked = soldSeatIds.Contains(s.SeatId)
                        ? true         // đã bán
                        : heldSeatIds.Contains(s.SeatId)
                            ? true     // đang giữ tạm
                            : false // trống
                })
                .ToListAsync();

            return Ok(seats);
        }
    }
}
