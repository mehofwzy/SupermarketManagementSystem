using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SM_API.Attributes;
using SM_API.Attributes.SM_API.Attributes;
using SM_API.Data;
using SM_API.Models;
using SM_API.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        //[JWTHasPermission("view_products")]
        [HasPermission("Product", "View")]

        public async Task<IActionResult> GetProducts(
            [FromQuery] string? code,
            [FromQuery] string? name,
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? price,
            [FromQuery] int? stockQuantity,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var query = _context.Products
                .Include(b => b.Category)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(code))
                query = query.Where(b => b.Code.Contains(code));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.Contains(name));


            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);

            if (price.HasValue)
                query = query.Where(b => b.Price == price.Value);

            if (stockQuantity.HasValue)
                query = query.Where(b => b.StockQuantity >= stockQuantity.Value);

            // Pagination
            var totalRecords = await query.CountAsync();

            var products = await query
                .OrderByDescending(b => b.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .Select(e=> new ProductDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    CategoryId = e.CategoryId,
                    Code = e.Code,
                    Price = e.Price,
                    StockQuantity = e.StockQuantity
                }).ToListAsync();


            return Ok(new
            {
                Products = products,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Select(e => new ProductDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    CategoryId = e.CategoryId,
                    Code = e.Code,
                    Price = e.Price,
                    StockQuantity = e.StockQuantity

                }).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = productDto.Name,
                Code= productDto.Code,
                Price=productDto.Price,
                StockQuantity=productDto.StockQuantity,
                CategoryId= new Guid("9E16B584-427F-479C-B54A-8EB7C6541920")
            };

            // Check if Category exists
            var category = await _context.ProductCategories.FindAsync(product.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid Category ID.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        //// PUT: api/products/{id}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { Message = "product not found" });

            product.Name = productDto.Name;
            product.Code = productDto.Code;
            product.Price = productDto.Price;
            product.CategoryId = new Guid("9E16B584-427F-479C-B54A-8EB7C6541920"); //productDto.CategoryId;
            product.StockQuantity = productDto.StockQuantity;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "product updated successfully" });
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if product exists in any BillItem
            bool isProductInBill = await _context.BillItems.AnyAsync(bi => bi.ProductId == id);
            if (isProductInBill)
            {
                return BadRequest("Cannot delete product because it exists in a bill.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
