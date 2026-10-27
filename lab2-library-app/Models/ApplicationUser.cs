using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lab2_library_app.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Navigation property: A user can have multiple loans
        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}