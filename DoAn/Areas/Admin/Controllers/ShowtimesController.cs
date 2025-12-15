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
        private readonly IDbContextFactory _dbFactory;
        public ShowtimesController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Index(int? branchId, int page = 1)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            int pageSize = 10;
            var branches = db.Branches.ToList();
            ViewBag.Branches = branches;
            ViewBag.SelectedBranchId = branchId;

            var query = db.Showtimes
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
                .OrderByDescending(s => s.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return View(showtimes);
        }

        public async Task<IActionResult> Create(int? branchId)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            ViewBag.SelectedBranchId = branchId;

            var query = db.Rooms.AsQueryable();
            if (branchId != null)
            {
                query = query.Where(r => r.BranchId == branchId);
            }
            var vm = new ShowtimeCreateViewModel()
            {
                Showtime = new Models.Booking.Showtime(),
                Movies = await db.Movies.ToListAsync(),
                Branches = await db.Branches.ToListAsync(),
                Rooms = query.ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(ShowtimeCreateViewModel model)
        {

            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(model.RoomId);
            //Console.WriteLine(model.MovieId);
            //Console.WriteLine(model.StartTime);
            //Console.WriteLine(model.EndTime);

            ModelState.Remove("Movies");
            ModelState.Remove("Branches");
            ModelState.Remove("Rooms");
            ModelState.Remove("Showtime");

            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            try
            {
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Làm mới");
                    // load lại dropdown nếu có lỗi
                    model.Movies = db.Movies.ToList();
                    model.Branches = db.Branches.ToList();
                    model.Rooms = db.Rooms.Where(r => r.BranchId == model.BranchId).ToList();
                    return View(model);
                }

                var showtime = new Showtime
                {
                    MovieId = model.MovieId,
                    RoomId = model.RoomId,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };


                db.Showtimes.Add(showtime);
                db.SaveChanges();
                Console.WriteLine("Thêm suất chiếu thành công!");
                Console.ResetColor();
                TempData["success"] = "Thêm suất chiếu thành công!";
                return RedirectToAction("Index", "Showtimes");
            }
            catch (Exception ex)
            {
                var dbError = ex.InnerException?.InnerException?.Message
                       ?? ex.InnerException?.Message
                       ?? ex.Message;

                model.Movies = db.Movies.ToList();
                model.Branches = db.Branches.ToList();
                model.Rooms = db.Rooms.Where(r => r.BranchId == model.BranchId).ToList();
                Console.WriteLine("[Create Showtime]: " + dbError);
                ViewBag.Error = dbError;
                return View(model);
            }
        }

        public IActionResult Edit(int showtimeId)
        {

            return View();
        }
    }
}
