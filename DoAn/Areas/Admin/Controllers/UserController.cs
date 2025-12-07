using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class UserController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public UserController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var users = db.Users.Take(15).ToList();
            return View(users);
        }
    }
}
