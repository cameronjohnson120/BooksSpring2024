﻿namespace BooksSpring2024.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<Cart> CartItems { get; set; }


        public Order Order { get; set; }
    }
}
