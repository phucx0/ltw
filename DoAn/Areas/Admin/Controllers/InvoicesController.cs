using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class InvoicesController : Controller
    {
        private ModelContext _context;
        
        public InvoicesController(ModelContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status, string? method, int page = 1)
        {
            int pageSize = 10;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedMethod = method;

            var query = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Branch)
                .Include(p => p.Promotion)
                .AsQueryable();

            // Filter theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }

            // Filter theo phương thức thanh toán
            if (!string.IsNullOrEmpty(method))
            {
                query = query.Where(p => p.Method == method);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            var payments = await query
                .OrderByDescending(p => p.PaymentTime ?? p.Booking.BookingTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(payments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Branch)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                .Include(p => p.Promotion)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }
    }
}

