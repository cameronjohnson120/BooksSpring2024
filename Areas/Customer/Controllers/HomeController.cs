using BooksSpring2024.Data;
using BooksSpring2024.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BooksSpring2024.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private BooksDBContext _dbContext;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, BooksDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //the letter c is essentially a book
            var listOfBooks = _dbContext.Books.Include(c => c.Category);
            return View(listOfBooks.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int ID)
        {
            Book book = _dbContext.Books.Find(ID); //fetch the book from the database 

            _dbContext.Books.Entry(book).Reference(b => b.Category).Load(); //go to the reference object book and load the object with the category information as well 
            //create cart object 
            var cart = new Cart
            {
                BookID = ID,
                Book = book,
                Quantity = 1
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize] //authorize user before running the action method, i.e. need to be logged in 
        public IActionResult Details(Cart cart)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); //fetches the userID
            cart.UserID = userID;
            Cart existingCart = _dbContext.Carts.FirstOrDefault(c=> c.UserID == userID && c.BookID == cart.BookID);

            if(existingCart != null)//cart exists
            {
                //update the cart
                existingCart.Quantity += cart.Quantity;
                _dbContext.Carts.Update(existingCart);

            }
            else
            {
                _dbContext.Carts.Add(cart);
            }
           
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

