using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SM_API.Attributes;
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
    //[Authorize]
    //[Authorize(Roles = "Admin, emp")]

    public class ClientsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ClientsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<IActionResult> GetClients(
            [FromQuery] string? name,
            [FromQuery] string? Address,
            [FromQuery] string? PhoneNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var query = _context.Clients
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.Contains(name));

            if (!string.IsNullOrEmpty(Address))
                query = query.Where(b => b.Address.Contains(Address));

            if (!string.IsNullOrEmpty(PhoneNumber))
                query = query.Where(b => b.PhoneNumber.Contains(PhoneNumber));


            // Pagination
            var totalRecords = await query.CountAsync();

            var clients = await query
                .OrderByDescending(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ClientDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Address = e.Address,
                    Balance = e.Balance,
                    PhoneNumber=e.PhoneNumber
                }).ToListAsync();

            return Ok(new
            {
                Clients = clients,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/clients/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await _context.Clients
            .Select(e => new ClientDto
            {
                Id = e.Id,
                Name = e.Name,
                Address = e.Address,
                Balance = e.Balance,
                PhoneNumber = e.PhoneNumber

            }).FirstOrDefaultAsync(p => p.Id == id);

            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient([FromBody] ClientDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = new Client
            {
                Name = clientDto.Name,
                Address = clientDto.Address,
                Balance = (decimal)00.00,
                PhoneNumber = clientDto.PhoneNumber,
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // PUT: api/clients/{id}
        [AuthorizePermission("edit")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] ClientDto clientDto)
        {

            //var permissions = User.Claims
            //              .Where(c => c.Type == "permission")
            //              .Select(c => c.Value)
            //              .ToList();

            //if (!permissions.Contains("edit"))
            //    return Forbid("You don't have permission to edit.");

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound(new { Message = "client not found" });

            client.Name = clientDto.Name;
            client.Address = clientDto.Address;
            client.PhoneNumber = clientDto.PhoneNumber;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "client updated successfully" });

        }

        // DELETE: api/clients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
