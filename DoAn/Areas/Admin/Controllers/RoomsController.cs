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
        private ModelContext _context;
        public RoomsController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var rooms = _context.Rooms
                .Include(r => r.Branch)
                .Include(r => r.RoomType)
                .Take(10)
                .ToList();
            return View(rooms);
        }
    }
}
