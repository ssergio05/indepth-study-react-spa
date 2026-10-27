using lab2_library_app.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab2_library_app.Controllers;

[ApiController]
[Route("api/books")]
public class BooksApiController : ControllerBase
{
    private readonly lab2_library_appContext _context;

    public BooksApiController(lab2_library_appContext context)
    {
        _context = context;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await _context.Book
            .Include(b => b.Genre)
            .AsNoTracking()
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.Author,
                b.GenreId,
                b.Genre != null ? b.Genre.Name : null))
            .ToListAsync();

        return Ok(books);
    }

    public record BookDto(int Id, string Title, string Author, int GenreId, string? GenreName);
}
