using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class BranchesController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        public BranchesController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var branches = db.Branches
                .Take(10)
                .ToList();
            return View(branches);
        }
    }
}
