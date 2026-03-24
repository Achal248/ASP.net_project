<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;

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
=======
﻿using ColorFill.Data;
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
>>>>>>> main
        }
    }
}
