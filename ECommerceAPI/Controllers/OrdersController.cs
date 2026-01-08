using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string? productName)
        {
            var ordersQuery = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                ordersQuery = ordersQuery.Where(o => o.Product.Name.Contains(productName));
            }

            var orders = await ordersQuery
                .Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    o.ProductId,
                    o.Quantity,
                    ProductName = o.Product != null ? o.Product.Name : "N/A",
                    FullName = o.User != null ? o.User.FirstName + " " + o.User.LastName : "Unknown User",
                    o.OrderDate,
                    o.TotalAmount,
                    o.ShippingAddress,
                    o.Status
                })
                .ToListAsync();

            return Ok(orders);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] UpdateOrderDto request)
        {
            var role = Request.Headers["UserRole"].ToString();
            if (string.IsNullOrEmpty(role)) return StatusCode(401, new { message = "Please log in." });

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) return NotFound(new { message = "Product not found" });

            if (product.Stock < request.Quantity) 
                return BadRequest(new { message = $"Not enough stock. Remaining: {product.Stock}" });

            product.Stock -= request.Quantity;

            var order = new Order
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                ShippingAddress = request.ShippingAddress,
                OrderDate = DateTime.Now,
                TotalAmount = product.Price * request.Quantity,
                Status = string.IsNullOrEmpty(request.Status) ? "Pending" : request.Status
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully!", orderId = order.OrderId });
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            // Security Headers (Similar to Delete)
            var role = Request.Headers["UserRole"].ToString().Trim();
            var userIdHeader = Request.Headers["UserId"].ToString().Trim();
            int.TryParse(userIdHeader, out int loggedInUserId);

            // 1. Find the existing order
            var order = await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound(new { message = "Order not found" });

            // 2. Security: Only Admin or the Order Owner can update
            if (!role.Equals("admin", StringComparison.OrdinalIgnoreCase) && order.UserId != loggedInUserId)
                return StatusCode(403, new { message = "Unauthorized to update this order." });

            // 3. Logic: If Product or Quantity changes, adjust stock and TotalAmount
            bool isProductChanged = dto.ProductId > 0 && dto.ProductId != order.ProductId;
            bool isQuantityChanged = dto.Quantity > 0 && dto.Quantity != order.Quantity;

            if (isProductChanged || isQuantityChanged)
            {
                // Restore stock to the OLD product
                var oldProduct = await _context.Products.FindAsync(order.ProductId);
                if (oldProduct != null) oldProduct.Stock += order.Quantity;

                // Set target values
                var targetProductId = dto.ProductId > 0 ? dto.ProductId : order.ProductId;
                var targetQuantity = dto.Quantity > 0 ? dto.Quantity : order.Quantity;

                // Fetch NEW product
                var newProduct = await _context.Products.FindAsync(targetProductId);
                if (newProduct == null) return NotFound(new { message = "New product not found" });

                // Check stock on new product
                if (newProduct.Stock < targetQuantity)
                {
                    // Rollback: put stock back to old product if we fail
                    if (oldProduct != null) oldProduct.Stock -= order.Quantity; 
                    return BadRequest(new { message = "Insufficient stock for the new selection." });
                }

                // Deduct stock and update price
                newProduct.Stock -= targetQuantity;
                order.ProductId = targetProductId;
                order.Quantity = targetQuantity;
                order.TotalAmount = newProduct.Price * targetQuantity;
            }

            // 4. Update other fields
            if (!string.IsNullOrEmpty(dto.ShippingAddress)) order.ShippingAddress = dto.ShippingAddress;
            if (!string.IsNullOrEmpty(dto.Status)) order.Status = dto.Status;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Order updated successfully", orderId = id });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Database error" });
            }
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var role = Request.Headers["UserRole"].ToString().Trim();
            var userIdHeader = Request.Headers["UserId"].ToString().Trim();

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(userIdHeader))
                return BadRequest(new { message = "Security headers missing." });

            if (!int.TryParse(userIdHeader, out int loggedInUserId))
                return BadRequest(new { message = "Invalid UserId header." });

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });

            if (!role.Equals("admin", StringComparison.OrdinalIgnoreCase) && order.UserId != loggedInUserId)
                return StatusCode(403, new { message = "Unauthorized." });

            var product = await _context.Products.FindAsync(order.ProductId);
            if (product != null) product.Stock += order.Quantity;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted and stock restored." });
        }
    }
}