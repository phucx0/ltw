using DoAn.Areas.Admin.ViewModels;
using DoAn.Models.Booking;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,manager")]
    public class ShowtimesController : Controller
    {
        private ModelContext _context;
        public ShowtimesController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? branchId, int page = 1)
        {
            int pageSize = 10;
            var branches = _context.Branches.ToList();
            ViewBag.Branches = branches;
            ViewBag.SelectedBranchId = branchId;

            var query = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Branch)
                .AsQueryable();

            if (branchId != null)
            {
                query = query.Where(s => s.Room.Branch.BranchId == branchId);
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            var showtimes = query
                .OrderBy(s => s.ShowtimeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return View(showtimes);
        }

        public async Task<IActionResult> Create(int? branchId) 
        {
            //var branchs = await _context.Branches.ToListAsync();
            //ViewBag.Branchs = branchs;
            ViewBag.SelectedBranchId = branchId;


            var query = _context.Rooms.AsQueryable();
            if (branchId != null)
            {
                query = query.Where(r => r.BranchId == branchId);
            }
            var vm = new ShowtimeCreateViewModel()
            {
                Showtime = new Models.Booking.Showtime(),
                Movies = await _context.Movies.ToListAsync(),
                Branches = await _context.Branches.ToListAsync(),
                Rooms = query.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(ShowtimeCreateViewModel model)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(model.RoomId);
            Console.WriteLine(model.MovieId);
            Console.WriteLine(model.StartTime);
            Console.WriteLine(model.EndTime);

            ModelState.Remove("Movies");
            ModelState.Remove("Branches");
            ModelState.Remove("Rooms");
            ModelState.Remove("Showtime");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Làm mới");
                // load lại dropdown nếu có lỗi
                model.Movies = _context.Movies.ToList();
                model.Branches = _context.Branches.ToList();
                model.Rooms = _context.Rooms.Where(r => r.BranchId == model.BranchId).ToList();
                return View(model);
            }

            var showtime = new Showtime
            {
                MovieId = model.MovieId,
                RoomId = model.RoomId,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };


            _context.Showtimes.Add(showtime);
            _context.SaveChanges();
            Console.WriteLine("Thêm suất chiếu thành công!");
            Console.ResetColor();
            TempData["success"] = "Thêm suất chiếu thành công!";
            return RedirectToAction("Index", "Showtimes");
        }

        public IActionResult Edit(int showtimeId)
        {

            return View();
        }
    }
}
