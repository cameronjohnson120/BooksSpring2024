using Microsoft.AspNetCore.Mvc;
using BooksSpring2024.Models;


using BooksSpring2024.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using BooksSpring2024.Models.NewFolder;

namespace BooksSpring2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private BooksDBContext _dbContext; //context variable to use as a reference to the database 
        private IWebHostEnvironment _environment; //capability to loook at project, folders in that project
        public BookController(BooksDBContext dbContext, IWebHostEnvironment environment) //depency injection 
        {
            _dbContext = dbContext; //holds reference to object, allows to talk to database 
            _environment = environment;
        }
        public IActionResult Index() //default view, shows list of all books
        {
            var listOfBooks = _dbContext.Books.ToList();
            return View(listOfBooks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //fetch the categories and then display them on the form as a drop down
            //select list item is the data type of the enumerable variables 
            //on the right hand side we are fetching the list of categories from the database, converting it to a list 
            //then using projection to project this list into SelectListItem type 
            //=> is known as the lambda expresion, uses an input from the left to make a new SelectListItem
            //PROJECTION: allows us to project a category object to a SelectListItem object,
            //where the name of the category is used as the text and the categoryID is used as the value in the SelectListItem

            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.CategoryID.ToString()
            });

            //1)ViewBag - dynamic element that allows us to put smaller elements of data in it, allows to pass data from controller to the view
            //a wrapper around the viewdata
            //ViewBag.ListOfCategories = listOfCategories;    


            //2)ViewData - allows to pass data from the controller to the view, not dynamic like viewbag
            //ViewData["ListOfCategoriesVD"] = listOfCategories;

            //3)ViewModel - more complex than the other two, allows to show multiple models together, specifically created to support a complex view
            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();
            bookWithCategoriesVMobj.Book = new Book();
            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;

            return View(bookWithCategoriesVMobj);
        }
        [HttpPost]
        public IActionResult Create(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile? imgFile) //include the file and the book model 
        {
            if (ModelState.IsValid) //validate that the input matches the current model
            {
                string wwwrootPath = _environment.WebRootPath; //takes up until the root folder
                if (imgFile != null)
                {
                    //using fileStream, allows to talk to project folders and add things to those file folders 
                    //a slash within double quotes - the compiller assumes an escape sequence, add @ before file string 
                    using (var fileStream = new FileStream(Path.Combine(wwwrootPath, @"Images\bookImages\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream); //saves the file in the specified folder
                    }
                    bookWithCategoriesVMobj.Book.ImgURL = @"\Images\BookImages\" + imgFile.FileName;
                }

                _dbContext.Books.Add(bookWithCategoriesVMobj.Book); //adding the new object to the table
                _dbContext.SaveChanges(); //applying the changes to the table

                return RedirectToAction("Index", "Book"); //redirect user to the book index view
            }
            return View(bookWithCategoriesVMobj); //if the model isn't valid, the user will be returned
        }

        //[HttpGet]
        //public IActionResult Edit(int ID)
        //{
        //    //find the record within the database
        //    BookWithCategoriesVM bookObj = _dbContext.Books.Find(ID);
        //    return View(bookObj);
        //}


        //[HttpPost]
        //public IActionResult Edit(int ID, [Bind("BookTitle, Author, Description, Price, CategoryID, ImgURL")] BookWithCategoriesVM bookObj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _dbContext.Books.Update(bookObj.Book);
        //        _dbContext.SaveChanges();

        //        return RedirectToAction("Index", "Book");
        //    }
        //    return View(bookObj); //displays the form for the user to check again and fix 
        //}


        [HttpGet]
        public IActionResult Edit(int ID)
        {
            //find the record within the database
            Book bookObj = _dbContext.Books.Find(ID);
            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.CategoryID.ToString()
            });
            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();
            bookWithCategoriesVMobj.Book = new Book();
            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;

            return View(bookWithCategoriesVMobj);
        }


        [HttpPost]
        public IActionResult Edit(BookWithCategoriesVM bookWithCategoriesVM, IFormFile? imgFile)
        {
            string wwwrootPath = _environment.WebRootPath;
            if (ModelState.IsValid)
            {
                //checking if we have a new file to add
                if(imgFile != null)
                {
                    //checking if there is an existing file - if yes - delete it 
                    if( !string.IsNullOrEmpty(bookWithCategoriesVM.Book.ImgURL ))
                    {
                             var oldImgPath = Path.Combine(wwwrootPath, bookWithCategoriesVM.Book.ImgURL.TrimStart('\\')); //wwwrootPath ends with \ and the other begins with one, telling it to trim the leading back slash
                            if(System.IO.File.Exists(oldImgPath) )
                            {
                                 System.IO.File.Delete(oldImgPath);
                            }
                    }
                    using (var fileStream = new FileStream(Path.Combine(wwwrootPath, @"Images\bookImages\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream); //saves the file in the specified folder
                    }
                    bookWithCategoriesVM.Book.ImgURL = @"\images\bookImages\" + imgFile.FileName;
                }
                _dbContext.Books.Update(bookWithCategoriesVM.Book);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookWithCategoriesVM); //things did not work as expected
        }

        //failed attempt on edit
        //public IActionResult Edit(int ID, [Bind("BookTitle, Author, Description, Price, CategoryID, ImgURL")] Book bookObj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _dbContext.Books.Update(bookObj);
        //        _dbContext.SaveChanges();

        //        return RedirectToAction("Index", "Book");
        //    }
        //    return View(bookObj); //displays the form for the user to check again and fix 
        //}

    }
}
