using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class BranchesController : Controller
    {
        private ModelContext _context;
        public BranchesController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var branches = _context.Branches
                .Take(10)
                .ToList();
            return View(branches);
        }
    }
}
