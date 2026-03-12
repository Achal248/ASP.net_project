using Microsoft.AspNetCore.Mvc;
using ASP.net_project.Models;

namespace ASP.net_project.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            string fixedEmail = "admin@gmail.com";
            string fixedPassword = "12345";

            if (model.Email == fixedEmail && model.Password == fixedPassword)
            {
                HttpContext.Session.SetString("UserName", model.Email);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login");
        }
    }
}