using ASP.net_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.net_project.Controllers
{
    public class CategoryController : Controller
    {
        private static List<CategoryModel> categories = new List<CategoryModel>();

        public IActionResult Index()
        {
            return View(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(CategoryModel model)
        {
            categories.Add(model);
            return RedirectToAction("Index");
        }
    }
}
