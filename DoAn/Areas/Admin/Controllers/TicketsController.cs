using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models.Booking;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index()
        {
            List<Ticket> tickets = _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .Take(15).ToList();

            return View(tickets);
        }
    }
}
