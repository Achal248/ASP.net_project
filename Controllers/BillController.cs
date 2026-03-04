using Microsoft.AspNetCore.Mvc;

namespace ASP.net_project.Controllers
{
    public class BillController : Controller
    {
        // Static in-memory store for selected items
        // This will persist across pages for the user session
        private static List<string> AllSelectedItems = new List<string>();

        // GET: Bill page (from header)
        [HttpGet]
        public IActionResult Index()
        {
            // Send all selected items to view
            ViewBag.Paints = AllSelectedItems;
            return View();
        }

        // POST: Add to Bill button clicked
        [HttpPost]
        public IActionResult Index(string[] selectedPaints)
        {
            if (selectedPaints != null && selectedPaints.Length > 0)
            {
                // Add new items, avoid duplicates
                foreach (var paint in selectedPaints)
                {
                    if (!AllSelectedItems.Contains(paint))
                        AllSelectedItems.Add(paint);
                }
            }

            return RedirectToAction("Index");
        }

        // Optional: remove an item from bill page
        [HttpPost]
        public IActionResult Remove(string paintName)
        {
            if (!string.IsNullOrEmpty(paintName))
                AllSelectedItems.Remove(paintName);

            return RedirectToAction("Index");
        }
    }
}
