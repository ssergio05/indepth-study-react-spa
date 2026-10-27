using lab2_library_app.Data;
using lab2_library_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab2_library_app.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<lab2_library_app.Models.ApplicationUser> _userManager;
        private readonly lab2_library_appContext _context;

        public ProfileController(UserManager<lab2_library_app.Models.ApplicationUser> userManager, lab2_library_appContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userLoans = await _context.Loan
                .Include(l => l.Book)
                .Where(l => l.UserId == user.Id)
                .ToListAsync();

            ViewBag.Loans = userLoans;

            return View(user);
        }
    }
}