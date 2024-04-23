using BooksSpring2024.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BooksSpring2024.Data
{
    public class BooksDBContext : IdentityDbContext<IdentityUser>
    {
        public BooksDBContext(DbContextOptions<BooksDBContext> options)
            : base(options) 
        {
            
        }

        public DbSet<Category> Categories { get; set; } //corresponds to the sql table that will be created in the database, ach row will contain a catergory and the table will be called categories
        public DbSet<Book> Books { get; set; } //adds the books table to the database

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //method used to add seed data 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, Name = "Travel", Description = "This is the description for the travel category" },
                new Category { CategoryID = 2, Name = "Fiction", Description = "This is the description for the fiction category" },
                new Category { CategoryID = 3, Name = "Technology", Description = "This is the description for the technology category" }
                );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookID = 1, BookTitle = "The Wager", Author = "David Grann", Description = "A tale of shipwreck, mutiny, and murder.", Price = 19.99m, CategoryID = 1, ImgURL = "" },
                new Book { BookID = 2, BookTitle = "The Stand", Author = "Stephen King", Description = "Stephen King's longest book.", Price = 25.99m, CategoryID = 2, ImgURL = "" },
                new Book { BookID = 3, BookTitle = "Future Technology", Author = "Jeff Gold", Description = "About technology.", Price = 30.99m, CategoryID = 3, ImgURL = "" 
                });

               
        }
    }
}
