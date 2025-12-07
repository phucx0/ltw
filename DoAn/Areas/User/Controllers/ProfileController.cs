using DoAn.Controllers;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn.Areas.User.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public ProfileController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = db.Users
                .Include(u => u.Membership)
                    .ThenInclude(m => m.MembershipTier)
                .Include(u => u.Tickets.Where(t => t.Booking.Status == "confirmed"))
                    .ThenInclude(t => t.Booking)
                        .ThenInclude(b => b.Showtime)
                            .ThenInclude(s => s.Movie)
                .Include(u => u.Tickets.Where(t => t.Booking.Status == "confirmed"))
                    .ThenInclude(t => t.Seat)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Branch)
                .FirstOrDefault(u => u.UserId.ToString() == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }
    }
}
