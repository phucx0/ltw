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
        private ModelContext _context;
        public TicketsController (ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? status, int page = 1)
        {
            int pageSize = 10;
            ViewBag.SelectedStatus = status;

            var query = _context.Tickets
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
