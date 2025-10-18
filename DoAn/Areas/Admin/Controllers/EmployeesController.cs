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
        private ModelContext _context;
        public EmployeesController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var employees = _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "staff")
                .Take(10)
                .ToList();
            return View(employees);
        }
    }
}
