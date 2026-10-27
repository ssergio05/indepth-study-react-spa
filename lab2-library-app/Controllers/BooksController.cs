using lab2_library_app.Data;
using lab2_library_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

public class BooksController : Controller
{
    private readonly lab2_library_appContext _context;
    private readonly UserManager<lab2_library_app.Models.ApplicationUser> _userManager;

    public BooksController(lab2_library_appContext context, UserManager<lab2_library_app.Models.ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: BOOKS
    public async Task<IActionResult> Index(string searchString, int? searchGenre)
    {
        ViewBag.Genres = new SelectList(await _context.Genre.ToListAsync(), "Id", "Name");

        var books = from b in _context.Book
                    select b;

        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
        }

        if (searchGenre.HasValue)
        {
            books = books.Where(s => s.GenreId == searchGenre);
        }

        return View(await books.ToListAsync());
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewBag.Genres = new SelectList(
            _context.Genre,
            "Id",
            "Name");

        return View();
    }

    // POST: BOOKS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        Console.WriteLine($"Model State: {ModelState.IsValid}");
        return View(book);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: BOOKS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title")] Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: BOOKS/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Book.FindAsync(id);
        if (book != null)
        {
            _context.Book.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Book.Any(e => e.Id == id);
    }

    // GET: BOOKS/Loader
    [Authorize(Roles = "Admin")]
    public IActionResult Loader()
    {
        return View();
    }

    // POST: BOOKS/Loader
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Loader(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Please select a file.");
            return View();
        }

        using (var stream = new StreamReader(file.OpenReadStream()))
        {
            // Ignore the CSV header line
            string? header = await stream.ReadLineAsync();

            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');

                // Expected format: Title, Author, GenreId
                if (values.Length >= 3)
                {
                    var book = new Book
                    {
                        Title = values[0].Trim(),
                        Author = values[1].Trim(),
                        GenreId = int.Parse(values[2].Trim())
                    };
                    _context.Add(book);
                }
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: BOOKS/Borrow/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int id)
    {
        var book = await _context.Book.FindAsync(id);
        var user = await _userManager.GetUserAsync(User);

        if (book == null || user == null) return NotFound();

        bool isBorrowed = await _context.Loan.AnyAsync(l => l.BookId == id && l.ReturnDate == null);

        if (isBorrowed)
        {
            TempData["Error"] = "This book is already borrowed.";
            return RedirectToAction(nameof(Index));
        }

        var loan = new Loan
        {
            BookId = book.Id,
            UserId = user.Id,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(14)
        };

        _context.Add(loan);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: BOOKS/Return/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var loan = await _context.Loan
            .FirstOrDefaultAsync(l => l.BookId == id && l.UserId == user.Id && l.ReturnDate == null);

        if (loan != null)
        {
            loan.ReturnDate = DateTime.Now;
            _context.Update(loan);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Profile");
    }
}