using Microsoft.AspNetCore.Identity;
namespace lab2_library_app.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
