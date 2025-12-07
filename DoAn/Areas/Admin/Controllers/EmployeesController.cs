using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class EmployeesController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public EmployeesController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var employees = db.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "staff")
                .Take(10)
                .ToList();
            return View(employees);
        }
    }
}
