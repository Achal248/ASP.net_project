namespace ASP.net_project.Models
{
    public class BillItem
    {
        public string PaintName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
