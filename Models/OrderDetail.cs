using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksSpring2024.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        public int OrderID {  get; set; }
        [ForeignKey("OrderID")]
        [ValidateNever]
        public Order Order { get; set; }//navigational property
        public int BookID { get; set; }
        [ForeignKey("BookID")]
        [ValidateNever]
        public Book Book { get; set; }//navigational property

        public int Quantity { get; set; }
        public decimal Price { get; set; }



    }
}
