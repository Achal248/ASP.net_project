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
            string savedEmail = HttpContext.Session.GetString("RegEmail");
            string savedPassword = HttpContext.Session.GetString("RegPassword");

            if (model.Email == savedEmail && model.Password == savedPassword)
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

        [HttpPost]
        public IActionResult Register(string Name, string Email, string Phone, string Address, string Password)
        {
            HttpContext.Session.SetString("RegEmail", Email);
            HttpContext.Session.SetString("RegPassword", Password);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login");
        }
    }
}