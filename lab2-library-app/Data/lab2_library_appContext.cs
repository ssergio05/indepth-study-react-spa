using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using lab2_library_app.Models;

namespace lab2_library_app.Data;

public class lab2_library_appContext(DbContextOptions<lab2_library_appContext> options)
    : IdentityDbContext<Models.ApplicationUser>(options)
{
    public DbSet<Book> Book { get; set; } = default!;
    public DbSet<Genre> Genre { get; set; } = default!;
    public DbSet<Loan> Loan { get; set; } = default!;
}