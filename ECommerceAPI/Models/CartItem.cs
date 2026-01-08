namespace ECommerceAPI.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }  // Navigation property for Product
        public int UserId { get; set; }
        public User? User { get; set; }  // Navigation property for User
        public int Quantity { get; set; }

        public decimal TotalPrice => Product != null ? Product.Price * Quantity : 0;
    }
}
