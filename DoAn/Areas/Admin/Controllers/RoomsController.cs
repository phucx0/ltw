using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class RoomsController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        private ModelContext _context;
        public RoomsController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var rooms = db.Rooms
                .Include(r => r.Branch)
                .Include(r => r.RoomType)
                .Take(10)
                .ToList();
            return View(rooms);
        }
    }
}
