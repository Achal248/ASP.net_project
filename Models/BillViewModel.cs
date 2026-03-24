namespace ColorFill.Models
{
    public class BillViewModel
    {
        
            public string CustomerName { get; set; }
            public List<CartItem> CartItems { get; set; } = new List<CartItem>();
            public decimal GrandTotal { get; set; }
        
    }
}
