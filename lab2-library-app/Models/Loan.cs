using System;
using System.ComponentModel.DataAnnotations;

namespace lab2_library_app.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        // Navigation property
        public Book? Book { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation property linking to Identity User
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}