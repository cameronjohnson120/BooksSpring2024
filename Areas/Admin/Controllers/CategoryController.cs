using BooksSpring2024.Data;
using BooksSpring2024.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class CategoryController : Controller
    {
        private BooksDBContext _dbContext; //private variable which is an object of the booksDB class 

        public CategoryController(BooksDBContext dbContext) //dependancy injection 
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var listOfCategories = _dbContext.Categories.ToList();
            return View(listOfCategories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Category categoryObject)
        {
            //custom validation
            if (categoryObject.Name != null && categoryObject.Name.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Category name cannot be test");
            }
            //custom validation to make sure name and description arent the same 
            if (categoryObject.Name == categoryObject.Description)
            {
                ModelState.AddModelError("Description", "Name and description cannot be the same");

            }


            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(categoryObject);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Category");
            }
            return View(categoryObject); //displays the form for the user to check again and fix 
        }


        [HttpGet]
        public IActionResult Edit(int ID)
        {
            //find the record within the database
            Category category = _dbContext.Categories.Find(ID);
            return View(category);
        }


        [HttpPost]
        public IActionResult Edit(int ID, [Bind("CategoryID, Name, Description")] Category categoryObject)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(categoryObject);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Category");
            }
            return View(categoryObject); //displays the form for the user to check again and fix 
        }

        [HttpGet]
        public IActionResult Delete(int ID)
        {
            //find the record within the database
            Category category = _dbContext.Categories.Find(ID);
            return View(category);
        }


        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int ID)
        {
            Category categoryObject = _dbContext.Categories.Find(ID);
            _dbContext.Categories.Remove(categoryObject);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public IActionResult Details(int ID)
        {
            Category categoryObject = _dbContext.Categories.Find(ID);
            return View(categoryObject);
        }


    }
}





