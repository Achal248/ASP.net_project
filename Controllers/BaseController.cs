using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Filters;

namespace ColorFill.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("UserName");
            // If not logged in and not already in Account controller
            if (string.IsNullOrEmpty(user) &&
                !context.RouteData.Values["controller"].ToString().Equals("Account"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
