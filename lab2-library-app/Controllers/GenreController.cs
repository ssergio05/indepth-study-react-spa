using Microsoft.EntityFrameworkCore;
using lab2_library_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using lab2_library_app.Data;


namespace lab2_library_app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly lab2_library_appContext _context;
        public GenreController(lab2_library_appContext context)
        {
            _context = context;
        }

        // GET : Genre

        public async Task<IActionResult> Index()
        {
            return View(await _context.Genre.ToListAsync());
        }

        //GET : Genre/Create
        public IActionResult Create()
        {
            return View();
        }

        // Post : Genre/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET : Genre/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST : Genre/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var genre = await _context.Genre.FindAsync(id);
            if (genre != null)
            {
                _context.Genre.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
