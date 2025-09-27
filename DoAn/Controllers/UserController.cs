using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace DoAn.Controllers
{
    public class UserController : Controller
    {
        private readonly ModelContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(ModelContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Profile()
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

            _logger.LogInformation($"User loaded: {user.Membership.MembershipTier.TierName}");

            return View(user);
        }
    }
}
