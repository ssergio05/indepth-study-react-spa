using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lab2_library_app.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        // Foreign Key for Genre
        public int GenreId { get; set; }

        // Navigation property for Genre
        public Genre? Genre { get; set; }

        // Navigation property for Loans
        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}