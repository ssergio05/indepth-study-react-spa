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

            // Seed a default genre
            Console.WriteLine("Before the genre seeding");
            if (!await context.Genre.AnyAsync())
            {
                context.Genre.Add(new Genre
                {
                    Name = "Science Fiction"
                });
                Console.WriteLine("After the genre seeding");
                await context.SaveChangesAsync();
            }
        }
    }
}