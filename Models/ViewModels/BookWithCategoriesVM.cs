using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace BooksSpring2024.Models.NewFolder

{
    public class BookWithCategoriesVM
    {
        public Book Book { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ListOfCategories {get; set;}
    }
}
