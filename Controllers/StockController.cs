using Microsoft.AspNetCore.Mvc;

namespace ASP.net_project.Controllers
{
    public class StockController : Controller
    {
        // ===============================
        // MAIN CATEGORY PAGE
        // ===============================
        public IActionResult Index()
        {
            return View();
        }

        // ===============================
        // ADD INTERIOR PAINT - GET
        // ===============================
        [HttpGet]
        public IActionResult AddPaint()
        {
            return View();
        }

        // ===============================
        // ADD INTERIOR PAINT - POST
        // ===============================
        [HttpPost]
        public IActionResult AddPaint(
            string paintName,
            string brand,
            string category,
            int quantity,
            int price)
        {
            // TEMP DATA FOR INTERIOR
            TempData["PaintName"] = paintName;
            TempData["Brand"] = brand;
            TempData["Category"] = category;
            TempData["Quantity"] = quantity;
            TempData["Price"] = price;

            return RedirectToAction("Interior");
        }

        // ===============================
        // INTERIOR STOCK PAGE
        // ===============================
        public IActionResult Interior()
        {
            return View();
        }

        // ===============================
        // EXTERIOR STOCK PAGE
        // ===============================
        public IActionResult Exterior()
        {
            return View();
        }

        // ===============================
        // ADD EXTERIOR PAINT - GET
        // ===============================
        [HttpGet]
        public IActionResult AddExteriorPaint()
        {
            return View();
        }

        // ===============================
        // ADD EXTERIOR PAINT - POST
        // ===============================
        [HttpPost]
        public IActionResult AddExteriorPaint(
            string paintName,
            string brand,
            string category,
            int quantity,
            int price)
        {
            // TEMP DATA FOR EXTERIOR
            TempData["PaintName"] = paintName;
            TempData["Brand"] = brand;
            TempData["Category"] = category;
            TempData["Quantity"] = quantity;
            TempData["Price"] = price;

            return RedirectToAction("Exterior");
        }

        // ===============================
        // PUTTY STOCK PAGE
        // ===============================
        // ===============================
        // PUTTY STOCK PAGE
        // ===============================
        public IActionResult Putty()
        {
            return View();
        }

        // ===============================
        // ADD PUTTY - GET
        // ===============================
        [HttpGet]
        public IActionResult AddPutty()
        {
            return View();
        }

        // ===============================
        // ADD PUTTY - POST  ✅ ADD HERE
        // ===============================
        [HttpPost]
        public IActionResult AddPutty(
            string paintName,
            string brand,
            string category,
            int quantity,
            int price)
        {
            TempData["PuttyName"] = paintName;
            TempData["Brand"] = brand;
            TempData["Category"] = category;
            TempData["Quantity"] = quantity;
            TempData["Price"] = price;

            return RedirectToAction("Putty", "Stock");
        }
        // ===============================
        // PRIMER STOCK PAGE
        // ===============================
        [HttpGet]
        public IActionResult Primer()
        {

            return View();
        }

        // GET - show the Add Primer form
        [HttpGet]
        public IActionResult AddPrimer()
        {
            return View();
        }

        // POST - handle form submission
        [HttpPost]
        public IActionResult AddPrimer(
            string primerName,
            string brand,
            string category,
            int quantity,
            int price)
        {
            TempData["PrimerName"] = primerName;
            TempData["Brand"] = brand;
            TempData["Category"] = category;
            TempData["Quantity"] = quantity;
            TempData["Price"] = price;

            return RedirectToAction("Primer", "Stock");
        }
    }
}
