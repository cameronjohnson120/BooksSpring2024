using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksSpring2024.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }

        [DisplayName("Book Title")]
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public  int CategoryID { get; set; }

        public string? ImgURL { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }    //navigational property that connects this to the category table
    }
}
