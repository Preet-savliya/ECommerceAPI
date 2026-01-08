using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Select(p => new
                    {
                        p.ProductId,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.Stock,
                        CategoryName = p.Category != null ? p.Category.Name : "N/A",
                        p.CategoryId
                    })
                    .ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var role = Request.Headers["UserRole"].ToString().ToLower();
            if (string.IsNullOrEmpty(role) || role != "admin") 
                return StatusCode(403, new { message = "Only admins can add products." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Product added successfully", productId = product.ProductId });
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var role = Request.Headers["UserRole"].ToString().ToLower();
            if (role != "admin") return Forbid();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound(new { message = "Product not found" });

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.CategoryId = product.CategoryId;

            try 
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Product updated successfully" });
            }
            catch (DbUpdateConcurrencyException) 
            {
                return StatusCode(409, new { message = "Conflict: Product was modified elsewhere." });
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!Request.Headers.TryGetValue("UserRole", out var role) || role.ToString().ToLower() != "admin")
            {
                return StatusCode(403, new { message = "Access Denied: Only admins can delete products." });
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove any dependent orders and cart items before deleting the product
                var linkedOrders = _context.Orders.Where(o => o.ProductId == id);
                _context.Orders.RemoveRange(linkedOrders);

                var linkedCartItems = _context.CartItems.Where(c => c.ProductId == id);
                _context.CartItems.RemoveRange(linkedCartItems);

                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Product and related data deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting product");
                return BadRequest(new { message = "Database Error: " + ex.Message });
            }
        }
    }
}