// Models/UpdateOrderDto.cs
namespace ECommerceAPI.Models
{
    public class UpdateOrderDto
    {
        public int ProductId { get; set; }
        public int OrderId {get; set;}
        public int UserId {get; set;}
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Status {get; set;}
    }
}
