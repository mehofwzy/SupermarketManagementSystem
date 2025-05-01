using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class BillsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public BillsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET: api/bills
        [HttpGet]
        public async Task<IActionResult> GetBills(
            [FromQuery] Guid? clientId,
            [FromQuery] string? clientPhone,
            [FromQuery] int? billNumber,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] Guid? billTypeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var query = _context.Bills
                .Include(b => b.Client)
                .Include(b => b.BillType)
                .AsQueryable();

            // Filtering
            if (clientId.HasValue)
                query = query.Where(b => b.Client.Id == clientId);

            if (!string.IsNullOrEmpty(clientPhone))
                query = query.Where(b => b.Client.PhoneNumber.Contains(clientPhone));

            if (billNumber.HasValue)
                query = query.Where(b => b.BillNumber == billNumber.Value);

            if (fromDate.HasValue)
                query = query.Where(b => b.BillDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(b => b.BillDate <= toDate.Value);

            if (billTypeId.HasValue)
                query = query.Where(b => b.BillTypeId == billTypeId.Value);

            // Pagination
            var totalRecords = await query.CountAsync();

            var bills = await query
                .OrderByDescending(b => b.BillDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    BillDate = b.BillDate,
                    ClientId = b.ClientId,
                    ClientName = b.Client.Name,
                    ClientPhone = b.Client.PhoneNumber,
                    ClientAddress=b.Client.Address,
                    TotalAmount = b.TotalAmount,
                    BillNumber = b.BillNumber,
                    BillTypeId = b.BillTypeId,
                    BillTypeName = b.BillType.Name,
                    ItemsTotal = b.Items.Count,
                })
                .ToListAsync();

            return Ok(new
            {
                Bills = bills,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            });
        }


        // GET: api/bills/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBill(Guid id)
        {
            var bill = await _context.Bills
                .Include(e => e.Client)
                .Include(e => e.BillType)
                .Select(e => new BillDto
                {
                    Id = e.Id,
                    BillDate = e.BillDate,
                    ClientId = e.ClientId,
                    ClientName = e.Client.Name,
                    ClientPhone = e.Client.PhoneNumber,
                    ClientAddress = e.Client.Address,
                    TotalAmount = e.TotalAmount,
                    BillNumber = e.BillNumber,
                    BillTypeId = e.BillTypeId,
                    BillTypeName = e.BillType.Name,
                    ItemsTotal = e.Items.Count,
                    Items = e.Items.Select(item => new BillItemDto
                    {
                        Id = item.Id,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        BillId = item.Id,
                    }).ToList()


                })
                .FirstOrDefaultAsync(b => b.Id == id);


            if (bill == null)
            {
                return NotFound(new { Message = "Employee not found" });
            }

            return Ok(bill);
        }

        // POST: api/bills
        [HttpPost]
        public async Task<ActionResult<Bill>> CreateBill([FromBody] BillDto BillDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (BillDto.Items == null)
                return BadRequest(ModelState);

            //create the client if null
            if (BillDto.ClientId == null)
            {
                var createdClient = new Client
                {
                    Name = BillDto.ClientName,
                    Address = BillDto.ClientAddress,
                    PhoneNumber = BillDto.ClientPhone,
                    Balance = (decimal)00.00,
                };

                _context.Clients.Add(createdClient);
                await _context.SaveChangesAsync();

                BillDto.ClientId = createdClient.Id;
            }

            //calcluate bill number
            int currentMaxBillNumber = _context.Bills.Max(m => (int?)m.BillNumber) ?? 1110;

            var bill = new Bill
            {
                //BillDate= BillDto.BillDate != null ? BillDto.BillDate.Value : DateTime.Now,
                BillDate= DateTime.Now,
                ClientId = BillDto.ClientId.Value,
                BillTypeId= BillDto.BillTypeId,
                BillNumber = currentMaxBillNumber + 1,
                Items = BillDto.Items.Select(item => new BillItem
                {
                    Price = item.Price != null ? item.Price.Value : 0,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity != null ? item.Quantity.Value : 0,
                }).ToList()
            };

            decimal totalAmount = 0;
            foreach (var item in bill.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    return BadRequest($"Not enough stock for product {product.Name}.");
                }

                // Reduce stock
                product.StockQuantity -= item.Quantity;

                // Set price from product data
                item.Price = product.Price;
                item.BillId = bill.Id;

                // Calculate total
                totalAmount += item.Quantity * item.Price;
            }

            // Fetch the client
            var client = await _context.Clients.FindAsync(bill.ClientId);
            if (client == null)
            {
                return BadRequest("Client not found");
            }

            // Update client balance
            client.Balance = (client.Balance ?? 0) + totalAmount;
            bill.TotalAmount = totalAmount;

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
        }

        // DELETE: api/bills/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(Guid id)
        {
            var bill = await _context.Bills.Include(b => b.Items)
                                           .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(bill.ClientId);
            if (client == null)
            {
                return BadRequest("Client not found.");
            }

            // Restore client balance
            client.Balance = (client.Balance ?? 0) - bill.TotalAmount;

            // Restore stock
            foreach (var item in bill.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
