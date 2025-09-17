using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DoAn.Controllers
{
    public class UserController : Controller
    {
        private readonly ModelContext _context;
        public UserController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Profile(User user)
        {
            var users = _context.Users.ToList();
            foreach (var _user in users)
            {
                Console.WriteLine(_user.UserId);
            }
            return View(user);
        }
    }
}
