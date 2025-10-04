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
        private readonly ModelContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ModelContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _context.Users
                .Include(u => u.Membership)
                    .ThenInclude(m => m.MembershipTier)
                .Include(u => u.Tickets)
                .FirstOrDefault(u => u.UserId.ToString() == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            //_logger.LogInformation($"User loaded: {user.Membership.MembershipTier.TierName}");
            return View(user);
        }
    }
}
