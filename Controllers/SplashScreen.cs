using Microsoft.AspNetCore.Mvc;

namespace ASP.net_project.Controllers
{
    public class SplashScreenController : Controller
    {
        public IActionResult SplashScreen()
        {
            return View();
        }
    }
}