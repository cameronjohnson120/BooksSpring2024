using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksSpring2024.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int BookID { get; set; }
        [ForeignKey("BookID")]
        [ValidateNever]
        public Book Book { get; set; } //navigational property
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        [ValidateNever]
        public ApplicationUser AplicationUser { get; set; } //navigational property 

        public int Quantity { get; set; }

        [NotMapped]
        public decimal Subtotal { get; set; }
    }
}
