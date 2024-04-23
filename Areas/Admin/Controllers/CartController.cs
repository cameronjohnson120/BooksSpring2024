using BooksSpring2024.Data;
using BooksSpring2024.Models;
using BooksSpring2024.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BooksSpring2024.Areas.Admin.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private BooksDBContext _dbContext;

        public CartController(BooksDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); //fetches the userID

            var CartItemsList = _dbContext.Carts.Where(c => c.UserID == userID).Include(c => c.Book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = CartItemsList,
                Order = new Order()
            };

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Book.Price * cartItem.Quantity; //subtotal for individual cart item 
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal;
            }

            return View(shoppingCartVM);
        }

        public IActionResult IncrementByOne(int id)
        {
            Cart cart = _dbContext.Carts.Find(id);
            cart.Quantity++;
            _dbContext.Update(cart);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DecrementByOne(int id)
        {
            Cart cart = _dbContext.Carts.Find(id);

            if (cart.Quantity <= 1)
            {
                //remove the item
                _dbContext.Carts.Remove(cart);
                _dbContext.SaveChanges();
            }
            else
            {
                cart.Quantity--;
                _dbContext.Update(cart);
                _dbContext.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            Cart cart = _dbContext.Carts.Find(id);
            _dbContext.Carts.Remove(cart);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult ReviewOrder()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); //fetches the userID, goes to user object then finds the name identifier, using this variable
                                                                         //to then find the specific cart for that user in the next line

            var CartItemsList = _dbContext.Carts.Where(c => c.UserID == userID).Include(c => c.Book); //using book navigational property to pull in information
                                                                                                      //about the book as it is unavailable within just the cart
                                                                                                      //table, gives capability to show all information about
                                                                                                      //the book rather than simply just the book id
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = CartItemsList,

                Order = new Order()
            };

            //calculate order total
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Book.Price * cartItem.Quantity; //subtotal for individual cart item 
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal; //add recent subtotal to the cart total 
            }

            //fill in fields with user's information, reason for navigation property in cart model 
            shoppingCartVM.Order.ApplicationsUser = _dbContext.ApplicationUsers.Find(userID);

            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationsUser.Name;
            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationsUser.StreetAddress;
            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationsUser.City;
            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationsUser.State;
            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationsUser.PostalCode;
            shoppingCartVM.Order.Phone = shoppingCartVM.Order.ApplicationsUser.PhoneNumber;

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("ReviewOrder")]
        public IActionResult ReviewOrderPost(ShoppingCartVM shoppingCartVM)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); //fetches the userID, goes to user object then finds the name identifier, using this variable
                                                                         //to then find the specific cart for that user in the next line

            var CartItemsList = _dbContext.Carts.Where(c => c.UserID == userID).Include(c => c.Book); //using book navigational property to pull in information

            shoppingCartVM.CartItems = CartItemsList;

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Book.Price * cartItem.Quantity; //subtotal for individual cart item 
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal; //add recent subtotal to the cart total 
            }


            shoppingCartVM.Order.ApplicationsUser = _dbContext.ApplicationUsers.Find(userID);
            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationsUser.Name;
            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationsUser.StreetAddress;
            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationsUser.City;
            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationsUser.State;
            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationsUser.PostalCode;
            shoppingCartVM.Order.Phone = shoppingCartVM.Order.Phone;
            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            shoppingCartVM.Order.OrderStatus = "Pending";
            shoppingCartVM.Order.PaymentStatus = "Pending";

            _dbContext.Orders.Add(shoppingCartVM.Order);//creates a new order and generates an orderID which can then be used to ass OrderDetails

            _dbContext.SaveChanges();

            foreach( var eachCartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetail = new()
                {
                    OrderID = shoppingCartVM.Order.OrderID,
                    BookID = eachCartItem.BookID,
                    Quantity = eachCartItem.Quantity,
                    Price = eachCartItem.Book.Price
                };
                _dbContext.OrderDetails.Add(orderDetail);

            }
            _dbContext.SaveChanges();

            //StripeConfiguration.ApiKey = "sk_test_51P8kZWKSBDlyY37zZ2It2nHgXkKHFTkGhpkAYs2FQy9p6YFgfipaurf0WJhsddyplX4QUZfpoAENgRv5FmUAq1r300Qfz8M9Ab";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = "https://localhost:7032/" + $"customer/cart/orderconfirmation?id={shoppingCartVM.Order.OrderID}",

                CancelUrl = "https://localhost:7032/" + "customer/cart/index",

                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
            // {
            //     new Stripe.Checkout.SessionLineItemOptions
            //         {
            //             Price = "price_1MotwRLkdIwHu7ixYcPLm5uZ",
            //                Quantity = 2,
            //},
            // },
                Mode = "payment",
            };


            foreach (var eachCartItem in shoppingCartVM.CartItems)
            {
                //provide price, qty, and name

                //adding line items one by one
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        //needs to be of data type long
                        UnitAmount = (long)(eachCartItem.Book.Price * 100), //price of each item
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = eachCartItem.Book.BookTitle
                        }
                        
                    },
                    Quantity = eachCartItem.Quantity,
                };
                 options.LineItems.Add(sessionLineItem); //adding to set of line items within the foreach

            }

            //ask for service
            var service = new Stripe.Checkout.SessionService();
            //gives us a session where we can add the options above
            Session session = service.Create(options);

            //session ID created under the session variable, plugged into order object 
            shoppingCartVM.Order.SessionID = session.Id;

            _dbContext.SaveChanges();

            //redirect to session URL
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //becomes redundant after adding the stripe functionality and the return statement above 
            //return RedirectToAction("OrderConfirmation",new {id = shoppingCartVM.Order.OrderID });

        }

        public IActionResult OrderConfirmation(int id)
        {


            Order order = _dbContext.Orders.Find(id);

            var sessID = order.SessionID;

            //look at this session's status 
            var service = new SessionService();
            Session session = service.Get(sessID); //fetches the current session information 

            if(session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentIntentID = session.PaymentIntentId;
                order.PaymentStatus = "Approved";
            }

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Cart> listOfCartItems = _dbContext.Carts.ToList().Where(c => c.UserID == userID).ToList();
            _dbContext.Carts.RemoveRange(listOfCartItems);
            _dbContext.SaveChanges();

            return (View(id));
        }
    }


}
