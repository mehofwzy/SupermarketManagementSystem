using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SM_API.Attributes;
using SM_API.Data;
using SM_API.Models;
using SM_API.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public UserController(SupermarketDbContext context)
        {
            _context = context;
        }


        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? Username,
            [FromQuery] string? FullName,
            [FromQuery] string? Email,
            [FromQuery] string? Country,
            [FromQuery] string? Address,
            [FromQuery] string? PhoneNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var query = _context.Users
                .Include(b => b.CreatedBy)
                .Include(b => b.ModifiedBy)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(Username))
                query = query.Where(b => b.Username.Contains(Username));

            if (!string.IsNullOrEmpty(FullName))
                query = query.Where(b => b.FullName.Contains(FullName));

            if (!string.IsNullOrEmpty(Email))
                query = query.Where(b => b.Email.Contains(Email));

            if (!string.IsNullOrEmpty(Country))
                query = query.Where(b => b.Country.Contains(Country));

            if (!string.IsNullOrEmpty(Address))
                query = query.Where(b => b.Address.Contains(Address));

            if (!string.IsNullOrEmpty(PhoneNumber))
                query = query.Where(b => b.Phonenumber.Contains(PhoneNumber));


            // Pagination
            var totalRecords = await query.CountAsync();

            var users = await query
                .OrderByDescending(b => b.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new UserDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    FullName = e.FullName,
                    Email = e.Email,
                    PhoneNumber = e.Phonenumber,
                    Address = e.Address,
                    Country = e.Country,
                    Description = e.Description,
                    CreatedById = e.CreatedById,
                    CreatedByName =e.CreatedBy.FullName != null ? e.CreatedBy.FullName : "",
                    ModifiedById = e.ModifiedById,
                    ModifiedByName = e.ModifiedBy != null ? e.ModifiedBy.FullName : null,
                    CreationDate = e.CreationDate,
                    LastLogin = e.LastLogin,
                }).ToListAsync();

            return Ok(new
            {
                Users = users,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users
                .Include(b => b.CreatedBy)
                .Include(b => b.ModifiedBy)
                .Select(e => new UserDto
                 {
                     Id = e.Id,
                     Username = e.Username,
                     FullName = e.FullName,
                     Email = e.Email,
                     PhoneNumber = e.Phonenumber,
                     Address = e.Address,
                     Country = e.Country,
                     Description = e.Description,
                     CreatedById = e.CreatedById,
                     CreatedByName = e.CreatedBy.FullName != null ? e.CreatedBy.FullName : "",
                     ModifiedById = e.ModifiedById,
                     ModifiedByName = e.ModifiedBy != null ? e.ModifiedBy.FullName : null,
                     CreationDate = e.CreationDate,
                     LastLogin = e.LastLogin,
                 }).FirstOrDefaultAsync(p => p.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash,
                Email = userDto.Email,
                FullName = userDto.FullName,
                FisrtName = userDto.FisrtName,
                LastName = userDto.LastName,
                Phonenumber = userDto.PhoneNumber,
                Address = userDto.Address,
                Country = userDto.Country,
                Description = userDto.Description,
                CreatedById = userDto.CreatedById,
                CreationDate = DateTime.Now,
            };

            // Check if Username exists
            var UsernameExists = await _context.Users.AnyAsync(p => p.Username == userDto.Username);
            if (UsernameExists)
                return BadRequest("UsernameExists already exists");

            // Check if CreatedById exists
            var CreatedByIdExists = await _context.Users.FindAsync(userDto.CreatedById);
            if (CreatedByIdExists == null)
            {
                return BadRequest("Invalid CreatedById ID.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "user not found" });


            user.Email = userDto.Email;
            user.FullName = userDto.FullName;
            user.FisrtName = userDto.FisrtName;
            user.LastName = userDto.LastName;
            user.Phonenumber = userDto.PhoneNumber;
            user.Address = userDto.Address;
            user.Country = userDto.Country;
            user.Description = userDto.Description;
            user.ModifiedById = userDto.ModifiedById;

            // Check if ModifiedById exists
            var ModifiedByIdExists = await _context.Users.FindAsync(userDto.ModifiedById);
            if (ModifiedByIdExists == null)
            {
                return BadRequest("Invalid ModifiedById ID.");
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "user updated successfully" });
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}