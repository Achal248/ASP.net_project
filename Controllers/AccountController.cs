using ColorFill.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using SqlParameter = Microsoft.Data.SqlClient.SqlParameter;


namespace ColorFill.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbHelper _db;

        public AccountController(DbHelper db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string egit chekout mastremail, string password)
        {
            string query = "SELECT * FROM Users WHERE Email=@Email AND Password=@Password";

            SqlParameter[] parameters =
            {
            new SqlParameter("@Email", email),
            new SqlParameter("@Password", password)
        };

            DataTable dt = _db.ExecuteSelect(query, parameters);

            if (dt.Rows.Count > 0)
            {
                HttpContext.Session.SetString("UserName", dt.Rows[0]["FullName"].ToString());
                HttpContext.Session.SetString("UserEmail", dt.Rows[0]["Email"].ToString());

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login");

            ViewBag.Name = HttpContext.Session.GetString("UserName");
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");

            return View();
        }
    }
}

