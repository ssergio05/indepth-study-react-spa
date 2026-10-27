using Microsoft.AspNetCore.Identity;
using lab2_library_app.Models;
using Microsoft.EntityFrameworkCore;
using lab2_library_app.Data;

namespace lab2_library_app
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
    IServiceProvider serviceProvider,
    IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Models.ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<lab2_library_appContext>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@library.com";
            string? adminPassword = configuration["AdminPassword"];

            if (adminPassword != null &&
                await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Models.ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var createPowerUser = await userManager.CreateAsync(adminUser, adminPassword);

                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed default genres
            var defaultGenres = new[]
            {
                "Science Fiction",
                "Fantasy",
                "Mystery",
                "Historical Fiction",
                "Romance"
            };

            var existingGenreNames = await context.Genre
                .Select(g => g.Name)
                .ToListAsync();

            var missingGenres = defaultGenres
                .Where(name => !existingGenreNames.Contains(name))
                .Select(name => new Genre { Name = name })
                .ToList();

            if (missingGenres.Count > 0)
            {
                context.Genre.AddRange(missingGenres);
                await context.SaveChangesAsync();
            }

            // Seed sample books only when the catalog is empty
            if (!await context.Book.AnyAsync())
            {
                var genres = await context.Genre.ToDictionaryAsync(g => g.Name);

                context.Book.AddRange(
                    new Book { Title = "Dune", Author = "Frank Herbert", GenreId = genres["Science Fiction"].Id },
                    new Book { Title = "Neuromancer", Author = "William Gibson", GenreId = genres["Science Fiction"].Id },
                    new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", GenreId = genres["Fantasy"].Id },
                    new Book { Title = "Harry Potter and the Philosopher's Stone", Author = "J.K. Rowling", GenreId = genres["Fantasy"].Id },
                    new Book { Title = "The Name of the Wind", Author = "Patrick Rothfuss", GenreId = genres["Fantasy"].Id },
                    new Book { Title = "The Hound of the Baskervilles", Author = "Arthur Conan Doyle", GenreId = genres["Mystery"].Id },
                    new Book { Title = "Murder on the Orient Express", Author = "Agatha Christie", GenreId = genres["Mystery"].Id },
                    new Book { Title = "The Name of the Rose", Author = "Umberto Eco", GenreId = genres["Historical Fiction"].Id },
                    new Book { Title = "The Pillars of the Earth", Author = "Ken Follett", GenreId = genres["Historical Fiction"].Id },
                    new Book { Title = "Pride and Prejudice", Author = "Jane Austen", GenreId = genres["Romance"].Id }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}