using ColorFill.Data;
using ColorFill.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace ColorFill.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbHelper _db;

        public HomeController(ILogger<HomeController> logger, DbHelper db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        public IActionResult Dashboard()
        {
            // Existing stats query
            string query = @"
        SELECT 
            (SELECT COUNT(*) FROM Bill) AS TotalBills,
            (SELECT SUM(TotalAmount) FROM Bill) AS TotalSales,
            (SELECT SUM(BI.Quantity * P.CostPrice)
             FROM BillItem BI
             INNER JOIN Product P ON BI.ProductId = P.Id) AS TotalCost,
            (SELECT COUNT(*) FROM Product WHERE Quantity < 10) AS LowStockCount
    ";

            var dt = _db.ExecuteSelect(query);
            ViewBag.Data = dt.Rows[0];

            // Low stock items
            string lowStockQuery = "SELECT Name, Quantity FROM Product WHERE Quantity < 10";
            var lowStockTable = _db.ExecuteSelect(lowStockQuery);
            ViewBag.LowStockItems = lowStockTable;

            // --- New: Prepare Sales Trend (last 7 days) ---
            string salesTrendQuery = @"
    SELECT CONVERT(date, CreatedDate) AS SaleDate, SUM(TotalAmount) AS DailyTotal
    FROM Bill
    WHERE CreatedDate >= DATEADD(day, -6, GETDATE())
    GROUP BY CONVERT(date, CreatedDate)
    ORDER BY SaleDate
";
            var salesTrendTable = _db.ExecuteSelect(salesTrendQuery);

            // Convert to a list for Chart.js
            var salesTrendList = salesTrendTable.AsEnumerable()
                .Select(row => new {
                    Date = row.Field<DateTime>("SaleDate").ToString("yyyy-MM-dd"),
                    Total = row.Field<decimal>("DailyTotal")
                }).ToList();

            ViewBag.SalesTrend = salesTrendList;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
