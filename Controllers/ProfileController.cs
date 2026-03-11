using Microsoft.AspNetCore.Mvc;

namespace ASP.net_project.Controllers
{
    public class ProfileController : Controller
    {
        // PROFILE PAGE
        public IActionResult Index()
        {
            ViewBag.Name = TempData["Name"] ?? "Achal Gediya";
            ViewBag.Email = TempData["Email"] ?? "achal@email.com";
            ViewBag.Phone = TempData["Phone"] ?? "9999999999";
            ViewBag.Address = TempData["Address"] ?? "Rajkot, Gujarat";

            // Keep data for next request
            TempData.Keep();

            return View();
        }

        // EDIT PROFILE PAGE
        public IActionResult Edit()
        {
            ViewBag.Name = TempData["Name"] ?? "Achal Gediya";
            ViewBag.Email = TempData["Email"] ?? "achal@email.com";
            ViewBag.Phone = TempData["Phone"] ?? "9999999999";
            ViewBag.Address = TempData["Address"] ?? "Rajkot, Gujarat";

            TempData.Keep();
            return View();
        }

        // UPDATE PROFILE
        [HttpPost]
        public IActionResult Update(string name, string email, string phone, string address)
        {
            TempData["Name"] = name;
            TempData["Email"] = email;
            TempData["Phone"] = phone;
            TempData["Address"] = address;

            return RedirectToAction("Index");
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Account");
        }
    }
}