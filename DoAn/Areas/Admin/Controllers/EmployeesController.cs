using DoAn.Areas.Admin.ViewModels;
using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class EmployeesController : Controller
    {
        private readonly PasswordHasher<Models.Accounts.User> _passwordHasher = new PasswordHasher<Models.Accounts.User>();
        private readonly IDbContextFactory _dbFactory;

        public EmployeesController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HasPermission("VIEW_STAFF")]
        public IActionResult Index(string searchTerm, bool? isActive, string sortBy, int page = 1)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var query = db.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "staff");

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.FullName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.Phone.Contains(searchTerm));
            }

            // Lọc trạng thái
            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // Sắp xếp
            query = sortBy switch
            {
                "name_asc" => query.OrderBy(u => u.FullName),
                "name_desc" => query.OrderByDescending(u => u.FullName),
                "date_asc" => query.OrderBy(u => u.CreatedAt),
                "date_desc" => query.OrderByDescending(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            // Phân trang
            int pageSize = 10;
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var employees = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag
            ViewBag.SearchTerm = searchTerm;
            ViewBag.IsActive = isActive;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(employees);
        }

        [HasPermission("CREATE_STAFF")]
        [HttpGet]
        public IActionResult Create()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            ViewBag.Branches = db.Branches.ToList();
            return View();
        }

        [HasPermission("CREATE_STAFF")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                if (errors.Count > 0)
                {
                    Console.WriteLine($"Key: {key}");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  - {error.ErrorMessage}");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Error = string.Join("\n",
                     ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                );
                return View(model);
            }

            // Kiểm tra email đã tồn tại
            if (db.Users.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã được sử dụng!";
                ModelState.AddModelError("Email", "Email đã được sử dụng!");
                //ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (db.Users.Any(u => u.Phone == model.Phone))
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng!");
                ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }

            try
            {
                // Lấy role staff
                var staffRole = db.UserRoles.FirstOrDefault(r => r.RoleName == "staff");
                if (staffRole == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy role staff!");
                    ViewBag.Branches = db.Branches.ToList();
                    return View(model);
                }

                // Hash password
                var employee = new Models.Accounts.User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Birthday = model.Birthday,
                    RoleId = staffRole.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                    //Gender = model.Gender,
                    //Address = model.Address,
                    //BranchId = model.BranchId,
                };
                employee.PasswordHash = _passwordHasher.HashPassword(employee, model.Password);

                db.Users.Add(employee);
                db.SaveChanges();

                TempData["success"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }
        }

        [HasPermission("UPDATE_STAFF")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var employee = db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == id && u.Role.RoleName == "staff");

            if (employee == null)
            {
                TempData["error"] = "Không tìm thấy nhân viên!";
                return RedirectToAction("Index", "Employees", new { area = "Admin"});
            }

            var model = new EmployeeEditViewModel
            {
                UserId = employee.UserId,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                Birthday = employee.Birthday,
                //Gender = employee.Gender,
                //Address = employee.Address,
                //BranchId = employee.BranchId,
                IsActive = employee.IsActive ?? true
            };

            ViewBag.Branches = db.Branches.ToList();
            return View(model);
        }

        [HasPermission("UPDATE_STAFF")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("1");              
                ViewBag.Branches = db.Branches.ToList();
                TempData["error"] = string.Join("\n",
                     ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage)
                );
                return View(model);
            }

            var employee = db.Users.FirstOrDefault(u => u.UserId == model.UserId);
            if (employee == null)
            {
                TempData["error"] = "Không tìm thấy nhân viên!";
                return RedirectToAction("Index");
            }

            // Kiểm tra email trùng (trừ chính nó)
            if (db.Users.Any(u => u.Email == model.Email && u.UserId != model.UserId))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng!");
                ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }

            // Kiểm tra phone trùng (trừ chính nó)
            if (db.Users.Any(u => u.Phone == model.Phone && u.UserId != model.UserId))
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng!");
                ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }

            try
            {
                employee.FullName = model.FullName;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.Birthday = model.Birthday;
                //employee. = model.Gender;
                //employee.Address = model.Address;
                //employee.BranchId = model.BranchId;
                employee.IsActive = model.IsActive;

                db.SaveChanges();

                TempData["success"] = "Cập nhật nhân viên thành công!";
                return View(model);
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                ViewBag.Branches = db.Branches.ToList();
                return View(model);
            }
        }

        [HasPermission("DELETE_STAFF")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var employee = db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == id && u.Role.RoleName == "staff");

            if (employee == null)
            {
                TempData["error"] = "Không tìm thấy nhân viên!";
                return RedirectToAction("Index");
            }

            try
            {
                // Soft delete - chỉ đổi trạng thái
                employee.IsActive = false;
                db.SaveChanges();

                TempData["success"] = "Vô hiệu hóa nhân viên thành công!";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
