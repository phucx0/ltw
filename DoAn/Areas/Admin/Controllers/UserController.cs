using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class UserController : Controller
    {
        private ModelContext _context;
        public UserController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.Take(15).ToList();
            return View(users);
        }
    }
}
