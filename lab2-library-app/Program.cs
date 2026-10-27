using lab2_library_app;
using lab2_library_app.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("lab2_library_appContext") ?? throw new InvalidOperationException("Connection string 'lab2_library_appContext' not found.");


// Identity configuration
builder.Services.AddDefaultIdentity<lab2_library_app.Models.ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<lab2_library_appContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

string dataDirectory;
if (builder.Environment.IsDevelopment())
{
    dataDirectory = builder.Environment.ContentRootPath;
}
else
{
    var home = Environment.GetEnvironmentVariable("HOME");
    if (string.IsNullOrWhiteSpace(home))
        throw new InvalidOperationException("Azure HOME is not configured.");

    dataDirectory = Path.Combine(home, "data");
}

Directory.CreateDirectory(dataDirectory);
var connectionStringDB = $"Data Source={Path.Combine(dataDirectory, "app.db")};Pooling=False";

builder.Services.AddDbContext<lab2_library_appContext>(options =>
    options.UseSqlite(connectionStringDB));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<lab2_library_appContext>();
    db.Database.Migrate();
    // Use rollback journaling for this small app on shared file storage.
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE;");
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    try
    {
        await SeedData.InitializeAsync(services, configuration).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map Razor Pages for Identity UI
app.MapRazorPages();

app.Run();