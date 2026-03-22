using ColorFill.Data;
using ColorFill.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace ColorFill.Controllers
{
    public class BillingController : BaseController
    {
        private readonly DbHelper _db;

        public BillingController(DbHelper db)
        {
            _db = db;
        }

        // showing bill page
        public IActionResult Index()
        {
            var cart = GetCart();

            BillViewModel model = new BillViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Total)
            };

            return View(model);

        }

        // add to bill from stock 
        public IActionResult AddToBill(int id)
        {
            string query = "SELECT * FROM Product WHERE Id=@Id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            DataTable dt = _db.ExecuteSelect(query, parameters);

            if (dt.Rows.Count == 0)
                return RedirectToAction("Index");

            var row = dt.Rows[0];

            var cart = GetCart();

            int productId = Convert.ToInt32(row["Id"]);
            string name = row["Name"].ToString();
            decimal price = Convert.ToDecimal(row["SellingPrice"]);

            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = name,
                    SellingPrice = price,
                    Quantity = 1
                });
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // update the quantity 
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                return RedirectToAction("Index");

            if (quantity <= 0)
            {
                cart.Remove(item);
                SaveCart(cart);
                return RedirectToAction("Index");
            }

            int oldQuantity = item.Quantity;
            int difference = quantity - oldQuantity;

            // Get current stock from database
            string stockQuery = "SELECT Quantity FROM Product WHERE Id=@Id";

            SqlParameter[] stockParams =
            {
        new SqlParameter("@Id", productId)
    };

            DataTable dt = _db.ExecuteSelect(stockQuery, stockParams);

            if (dt.Rows.Count == 0)
                return RedirectToAction("Index");

            int stock = Convert.ToInt32(dt.Rows[0]["Quantity"]);

            // If increasing quantity
            if (difference > 0)
            {
                if (stock >= difference)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    TempData["Error"] = "Not enough stock available!";
                }
            }
            else
            {
                // Decreasing quantity is always allowed
                item.Quantity = quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // remove item 
        public IActionResult Remove(int productId)
        {
            var cart = GetCart();

            cart.RemoveAll(x => x.ProductId == productId);

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // save bill 
        [HttpPost]
        public IActionResult SaveBill(string customerName)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            int billId = 0;

            using (SqlConnection con = new SqlConnection(
                HttpContext.RequestServices
                .GetService<IConfiguration>()
                .GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();



                try
                {
                    string billNumber = "BILL-" + DateTime.Now.Ticks;

                    decimal grandTotal = cart.Sum(x => x.Total);

                    // Insert Bill
                    SqlCommand billCmd = new SqlCommand(@"
                        INSERT INTO Bill (BillNumber, CustomerName, TotalAmount)
                        OUTPUT INSERTED.Id
                        VALUES (@BillNumber, @CustomerName, @TotalAmount)", con, transaction);

                    billCmd.Parameters.AddWithValue("@BillNumber", billNumber);
                    billCmd.Parameters.AddWithValue("@CustomerName", customerName ?? "");
                    billCmd.Parameters.AddWithValue("@TotalAmount", grandTotal);

                    billId = (int)billCmd.ExecuteScalar();

                    foreach (var item in cart)
                    {
                        // Insert BillItem
                        SqlCommand itemCmd = new SqlCommand(@"
                            INSERT INTO BillItem (BillId, ProductId, Quantity, Price, Total)
                            VALUES (@BillId, @ProductId, @Quantity, @Price, @Total)", con, transaction);

                        itemCmd.Parameters.AddWithValue("@BillId", billId);
                        itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@Price", item.SellingPrice);
                        itemCmd.Parameters.AddWithValue("@Total", item.Total);
                        itemCmd.ExecuteNonQuery();

                        // Reduce Stock
                        SqlCommand stockCmd = new SqlCommand(@"
                            UPDATE Product 
                            SET Quantity = Quantity - @Qty
                            WHERE Id=@Id", con, transaction);

                        stockCmd.Parameters.AddWithValue("@Qty", item.Quantity);
                        stockCmd.Parameters.AddWithValue("@Id", item.ProductId);
                        stockCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Invoice", new { id = billId });
        }

        // session management for cart
        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(sessionData))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(sessionData);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(cart));
        }


        // invoice 


        public IActionResult Invoice(int id)
        {
            string billQuery = "SELECT * FROM Bill WHERE Id=@Id";
            SqlParameter[] billParam =
            {
        new SqlParameter("@Id", id)
    };

            DataTable billDt = _db.ExecuteSelect(billQuery, billParam);

            if (billDt.Rows.Count == 0)
                return RedirectToAction("Index");

            string itemQuery = @"
        SELECT P.Name, BI.Quantity, BI.Price, BI.Total
        FROM BillItem BI
        INNER JOIN Product P ON BI.ProductId = P.Id
        WHERE BI.BillId=@Id";

            DataTable itemDt = _db.ExecuteSelect(itemQuery, billParam);

            ViewBag.Bill = billDt.Rows[0];
            ViewBag.Items = itemDt;

            return View();
        }

        // sales report 
        public IActionResult SalesReport(DateTime? fromDate, DateTime? toDate)
        {
            string query = @"
        SELECT 
        COUNT(DISTINCT B.Id) AS TotalBills,
        SUM(B.TotalAmount) AS TotalSales,
        SUM(BI.Quantity * P.CostPrice) AS TotalCost
    FROM Bill B
    INNER JOIN BillItem BI ON B.Id = BI.BillId
    INNER JOIN Product P ON BI.ProductId = P.Id
    WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (fromDate.HasValue)
            {
                query += " AND CreatedDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
            }

            if (toDate.HasValue)
            {
                query += " AND CreatedDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", toDate.Value));
            }

            DataTable dt = _db.ExecuteSelect(query, parameters.ToArray());

            ViewBag.Report = dt.Rows[0];
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View();
        }

        //public IActionResult History()
        //{
        //    var bills = _db.GetAllBills();
        //    return View(bills);
        //}
    }
}
