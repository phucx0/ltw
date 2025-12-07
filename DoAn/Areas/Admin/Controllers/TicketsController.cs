using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models.Booking;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class TicketsController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public TicketsController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index(string? status, int page = 1)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            int pageSize = 10;
            ViewBag.SelectedStatus = status;

            var query = db.Tickets
                .Include(t => t.User)
                .Include(t => t.Seat)
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(t => t.Booking.Showtime)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .AsQueryable();

            // Filter theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            var tickets = query
                .OrderByDescending(t => t.BookingTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(tickets);
        }
    }
}
