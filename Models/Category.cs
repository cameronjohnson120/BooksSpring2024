using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace BooksSpring2024.Models
{
    public class Category

    {
        public int CategoryID { get; set; }
        [DisplayName("Category Name:")]
        [Required(ErrorMessage="The name for the category must be provided")]
        public string Name { get; set; }
        //question mark after data type makes the property nullable
        //server side validation
        [DisplayName("Category Description")]
        [Required(ErrorMessage ="The category description must be provided")]
        //[MaxLength(20, ErrorMessage = "Description cannot exceed 20 characters")]
        public string? Description { get; set; }


    }
}
