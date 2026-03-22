using ColorFill.Data;
using ColorFill.Models;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace ColorFill.Controllers
{
    public class StockController : BaseController
    {
        private readonly DbHelper _db;

        public StockController(DbHelper db)
        {
            _db = db;
        }

        // STOCK LIST + SEARCH + FILTER
        public IActionResult Index(string search, int? categoryId)
        {
            string query = @"
                SELECT p.*, c.Name AS CategoryName
                FROM Product p
                INNER JOIN Category c ON p.CategoryId = c.Id
                WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND p.Name LIKE @Search";
                parameters.Add(new SqlParameter("@Search", "%" + search + "%"));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query += " AND p.CategoryId = @CategoryId";
                parameters.Add(new SqlParameter("@CategoryId", categoryId.Value));
            }

            query += " ORDER BY p.CreatedDate DESC";

            DataTable dt = _db.ExecuteSelect(query, parameters.ToArray());

            List<Product> products = new List<Product>();

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    CategoryName = row["CategoryName"].ToString(),
                    CostPrice = Convert.ToDecimal(row["CostPrice"]),
                    SellingPrice = Convert.ToDecimal(row["SellingPrice"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    LowStockThreshold = Convert.ToInt32(row["LowStockThreshold"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            ViewBag.Categories = GetCategories();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;

            return View(products);
        }
        // Create product 
        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            string query = @"
                INSERT INTO Product
                (Name, CategoryId, CostPrice, SellingPrice, Quantity, LowStockThreshold)
                VALUES
                (@Name, @CategoryId, @CostPrice, @SellingPrice, @Quantity, @LowStockThreshold)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@CategoryId", product.CategoryId),
                new SqlParameter("@CostPrice", product.CostPrice),
                new SqlParameter("@SellingPrice", product.SellingPrice),
                new SqlParameter("@Quantity", product.Quantity),
                new SqlParameter("@LowStockThreshold", product.LowStockThreshold)
            };

            _db.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Index");
        }


        // Delete Product 

        public IActionResult Delete(int id)
        {
            string query = "DELETE FROM Product WHERE Id=@Id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            _db.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Index");
        }

        // Get category list 

        private List<Category> GetCategories()
        {
            string query = "SELECT * FROM Category ORDER BY Name";

            DataTable dt = _db.ExecuteSelect(query);

            List<Category> categories = new List<Category>();

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()
                });
            }

            return categories;
        }
    }
}
