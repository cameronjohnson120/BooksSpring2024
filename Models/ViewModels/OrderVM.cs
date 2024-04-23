namespace BooksSpring2024.Models.ViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
