using ColorFill.Data;
using ColorFill.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColorFill.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly DbHelper _db;

        public CategoryController(DbHelper db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string query = "SELECT * FROM Category ORDER BY CreatedDate DESC";
            DataTable dt = _db.ExecuteSelect(query);

            List<Category> categories = new List<Category>();

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            string query = "INSERT INTO Category (Name, Description) VALUES (@Name, @Description)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@Description", category.Description ?? (object)DBNull.Value)
            };

            _db.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            string query = "DELETE FROM Category WHERE Id=@Id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            _db.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Index");
        }
    }
}
