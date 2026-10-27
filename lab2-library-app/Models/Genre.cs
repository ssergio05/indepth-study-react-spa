using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lab2_library_app.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property: A genre can have multiple books
        public List<Book> Books { get; set; } = new List<Book>();
    }
}