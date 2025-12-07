using DoAn.Areas.Admin.ViewModels;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class DashboardController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public DashboardController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            // Thống kê tổng quan
            viewModel.TotalMovies = await db.Movies.CountAsync();
            viewModel.TotalBranches = await db.Branches.CountAsync();
            viewModel.TotalRooms = await db.Rooms.CountAsync();
            viewModel.TotalUsers = await db.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "user")
                .CountAsync();

            // Thống kê hôm nay
            viewModel.TodayShowtimes = await db.Showtimes
                .Where(s => s.StartTime.Date == today)
                .CountAsync();

            viewModel.TodayTicketsSold = await db.Tickets
                .Where(t => t.BookingTime.Date == today && t.Status == "booked")
                .CountAsync();

            viewModel.TodayRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value.Date == today &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // Thống kê tháng này
            viewModel.MonthlyRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value >= startOfMonth &&
                           p.PaymentTime.Value <= endOfMonth &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            viewModel.MonthlyTicketsSold = await db.Tickets
                .Where(t => t.BookingTime >= startOfMonth &&
                           t.BookingTime <= endOfMonth &&
                           t.Status == "booked")
                .CountAsync();

            return View(viewModel);
        }

        // API endpoints cho AJAX calls từ dashboard
        [HttpGet]
        public async Task<IActionResult> GetTopMovies()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var topMovies = await db.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Where(t => t.BookingTime >= startOfMonth && t.Status == "booked")
                .GroupBy(t => new
                {
                    t.Booking.Showtime.Movie.MovieId,
                    t.Booking.Showtime.Movie.Title,
                    t.Booking.Showtime.Movie.Genre
                })
                .Select(g => new
                {
                    MovieId = g.Key.MovieId,
                    Title = g.Key.Title,
                    Genre = g.Key.Genre,
                    TicketsSold = g.Count(),
                    Revenue = g.Sum(t => t.Price)
                })
                .OrderByDescending(x => x.TicketsSold)
                .Take(5)
                .ToListAsync();

            return Json(topMovies);
        }

        [HttpGet]
        public async Task<IActionResult> GetWeeklyRevenue()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Thứ 2

            var weeklyData = new List<object>();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                var revenue = await db.Payments
                    .Where(p => p.PaymentTime.HasValue &&
                               p.PaymentTime.Value.Date == date &&
                               p.Status == "paid")
                    .SumAsync(p => (decimal?)p.Amount) ?? 0;

                weeklyData.Add(new
                {
                    Day = date.ToString("ddd", new System.Globalization.CultureInfo("vi-VN")),
                    Revenue = revenue
                });
            }

            return Json(weeklyData);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            // Tính % thay đổi so với hôm qua
            var yesterdayRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value.Date == yesterday &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var todayRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value.Date == today &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // Tính % thay đổi so với tháng trước
            var lastMonthRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value >= startOfLastMonth &&
                           p.PaymentTime.Value <= endOfLastMonth &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var thisMonthRevenue = await db.Payments
                .Where(p => p.PaymentTime.HasValue &&
                           p.PaymentTime.Value >= startOfMonth &&
                           p.Status == "paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var lastMonthTickets = await db.Tickets
                .Where(t => t.BookingTime >= startOfLastMonth &&
                           t.BookingTime <= endOfLastMonth &&
                           t.Status == "booked")
                .CountAsync();

            var thisMonthTickets = await db.Tickets
                .Where(t => t.BookingTime >= startOfMonth &&
                           t.Status == "booked")
                .CountAsync();

            return Json(new
            {
                DailyRevenueChange = yesterdayRevenue > 0
                    ? ((todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100)
                    : 0,
                MonthlyRevenueChange = lastMonthRevenue > 0
                    ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100)
                    : 0,
                MonthlyTicketsChange = lastMonthTickets > 0
                    ? ((decimal)(thisMonthTickets - lastMonthTickets) / lastMonthTickets * 100)
                    : 0
            });
        }
    }
}
