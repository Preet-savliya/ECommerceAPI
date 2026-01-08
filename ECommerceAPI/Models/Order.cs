namespace ECommerceAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Status { get; set; }
    }
}