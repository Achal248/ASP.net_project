namespace ColorFill.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
