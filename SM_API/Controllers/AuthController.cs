using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SM_API.Data;
using SM_API.Models;
using SM_API.Models.Dtos;
using SM_API.Services;
using BCrypt.Net;
using System.Linq;
using System.Net;
using System.Security;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace SM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly SupermarketDbContext _context;
        private readonly IPermissionService _permissionService;


        public AuthController(IJwtService jwtService, SupermarketDbContext context, IPermissionService permissionService)
        {
            _jwtService = jwtService;
            _context = context;
            _permissionService = permissionService;
        }
        [HttpGet("checkUserPermission")]
        [Authorize]
        public async Task<IActionResult> CheckUserPermission(Guid userId, string objectName, string permissionName)
        {
            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, objectName, permissionName);
            return Ok(hasPermission);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Get user roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            //Get user toker
            var token = _jwtService.GenerateToken(user, roles);

            // Create refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = user.Id
            };

            //get oldTokens Before saving new refresh token
            var oldTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
                .ToListAsync();

            //revoke the previous oldTokens
            foreach (var old in oldTokens)
                old.IsRevoked = true;

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token,
                refreshToken = refreshToken.Token
            });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

            if (storedToken == null)
                return Unauthorized("Invalid or expired refresh token.");

            var user = storedToken.User;

            // Re-evaluate roles and permissions from DB
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();


            // Generate new token
            var newToken = _jwtService.GenerateToken(user, roles);

            // Create new refresh token
            var newTokenEntry = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = user.Id,
                ReplacedByToken = storedToken.Token
            };

            // Revoke old token
            storedToken.IsRevoked = true;

            _context.RefreshTokens.Add(newTokenEntry);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = newToken,
                refreshToken = newTokenEntry.Token

            });
        }

        //[HttpPost("RevokeToken")]
        //public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        //{
        //    var token = await _context.RefreshTokens
        //        .Include(rt => rt.User)
        //        .FirstOrDefaultAsync(rt => rt.Token == dto.Token);

        //    if (token == null || token.IsRevoked || token.Expires < DateTime.UtcNow)
        //        return BadRequest("Invalid or expired token");

        //    token.IsRevoked = true;
        //    await _context.SaveChangesAsync();

        //    return Ok("Token revoked successfully");
        //}

        //[HttpGet("roles")]
        //public async Task<IActionResult> GetRoles()
        //{
        //    var roles = await _context.Roles
        //        .OrderByDescending(b => b.Name)
        //        .Select(e => new RoleDto
        //        {
        //            Id = e.Id,
        //            Name = e.Name,
        //        }).ToListAsync();

        //    if (roles == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(roles);
        //}

        //[HttpPost("CreateRole")]
        //public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
        //{
        //    if (string.IsNullOrWhiteSpace(dto.Name))
        //        return BadRequest("Role name is required");

        //    var exists = await _context.Roles.AnyAsync(r => r.Name == dto.Name);
        //    if (exists)
        //        return BadRequest("Role already exists");

        //    var role = new Role { Id = Guid.NewGuid(), Name = dto.Name };
        //    _context.Roles.Add(role);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
        //}

        //[HttpPut("roles/{id}")]
        //public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDto dto)
        //{
        //    var role = await _context.Roles.FindAsync(id);
        //    if (role == null) return NotFound();

        //    role.Name = dto.Name;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "role updated successfully" });
        //}

        //[HttpDelete("roles/{id}")]
        //public async Task<IActionResult> DeleteRole(Guid id)
        //{
        //    var role = await _context.Roles.FindAsync(id);
        //    if (role == null) return NotFound();

        //    _context.Roles.Remove(role);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        ////permissions
        //[HttpGet("permissions")]
        //public async Task<IActionResult> GetPermissions()
        //{
        //    var permissions = await _context.Permissions
        //        .OrderByDescending(b => b.Name)
        //        .Select(e => new PermissionDto
        //        {
        //            Id = e.Id,
        //            Name = e.Name,
        //        }).ToListAsync();

        //    if (permissions == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(permissions);
        //}

        //[HttpPost("CreatePermission")]
        //public async Task<IActionResult> CreatePermission([FromBody] PermissionDto dto)
        //{
        //    if (string.IsNullOrWhiteSpace(dto.Name))
        //        return BadRequest("Permission name is required");

        //    var exists = await _context.Permissions.AnyAsync(p => p.Name == dto.Name);
        //    if (exists)
        //        return BadRequest("Permission already exists");

        //    var permission = new ObjectPermission { Id = Guid.NewGuid(), Name = dto.Name };
        //    _context.Permissions.Add(permission);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetPermissions), new { id = permission.Id }, permission);
        //}

        //[HttpPut("permissions/{id}")]
        //public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] PermissionDto dto)
        //{
        //    var permission = await _context.Permissions.FindAsync(id);
        //    if (permission == null) return NotFound();

        //    permission.Name = dto.Name;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "permission updated successfully" });
        //}

        //[HttpDelete("permissions/{id}")]
        //public async Task<IActionResult> DeletePermission(Guid id)
        //{
        //    var permission = await _context.Permissions.FindAsync(id);
        //    if (permission == null) return NotFound();

        //    _context.Permissions.Remove(permission);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpPost("assign-role")]
        //public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        //{
        //    var user = await _context.Users.FindAsync(dto.UserId);
        //    var role = await _context.Roles.FindAsync(dto.RoleId);

        //    if (user == null || role == null)
        //        return NotFound("User or Role not found");

        //    var alreadyAssigned = await _context.UserRoles
        //        .AnyAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

        //    if (alreadyAssigned)
        //        return BadRequest("Role already assigned to user");

        //    _context.UserRoles.Add(new UserRole
        //    {
        //        UserId = dto.UserId,
        //        RoleId = dto.RoleId
        //    });

        //    await _context.SaveChangesAsync();

        //    return Ok("Role assigned successfully");
        //}

        //[HttpDelete("remove-role")]
        //public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleDto dto)
        //{
        //    var userRole = await _context.UserRoles
        //        .FirstOrDefaultAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

        //    if (userRole == null)
        //        return NotFound("Role not assigned to this user.");

        //    _context.UserRoles.Remove(userRole);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpPost("assign-permission")]
        //public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDto dto)
        //{
        //    var role = await _context.Roles.FindAsync(dto.RoleId);
        //    var permission = await _context.Permissions.FindAsync(dto.PermissionId);

        //    if (role == null || permission == null)
        //        return NotFound("Role or Permission not found");

        //    var alreadyAssigned = await _context.RolePermissions
        //        .AnyAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId);

        //    if (alreadyAssigned)
        //        return BadRequest("Permission already assigned to role");

        //    _context.RolePermissions.Add(new RolePermission
        //    {
        //        RoleId = dto.RoleId,
        //        PermissionId = dto.PermissionId
        //    });

        //    await _context.SaveChangesAsync();

        //    return Ok("Permission assigned successfully");
        //}

        //[HttpDelete("remove-permission")]
        //public async Task<IActionResult> RemovePermissionFromRole([FromBody] RemovePermissionDto dto)
        //{
        //    var rolePermission = await _context.RolePermissions
        //        .FirstOrDefaultAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId);

        //    if (rolePermission == null)
        //        return NotFound("Permission not assigned to this role.");

        //    _context.RolePermissions.Remove(rolePermission);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpGet("{userId}/permissions")]
        //public async Task<IActionResult> GetUserPermissions(Guid userId)
        //{
        //    var user = await _context.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .ThenInclude(r => r.RolePermissions)
        //        .ThenInclude(rp => rp.Permission)
        //        .FirstOrDefaultAsync(u => u.Id == userId);

        //    if (user == null)
        //        return NotFound();

        //    var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        //    var permissions = user.UserRoles
        //        .SelectMany(ur => ur.Role.RolePermissions)
        //        .Select(rp => rp.Permission.Name)
        //        .Distinct()
        //        .ToList();

        //    return Ok(new
        //    {
        //        Roles = roles,
        //        Permissions = permissions
        //    });
        //}


        //[HttpPost("old1_login")]
        //public IActionResult old1_login([FromBody] LoginDto dto)
        //{
        //    // Replace this with your actual DB lookup
        //    if (dto.Username == "admin" && dto.Password == "1234")
        //    {
        //        string token = _jwtService.Old_GenerateToken("1", "admin", "Admin");
        //        return Ok(new { token });
        //    }
        //    else if (dto.Username == "emp" && dto.Password == "1234")
        //    {
        //        string token = _jwtService.Old_GenerateToken("2", "emp", "emp");
        //        return Ok(new { token });
        //    }

        //    return Unauthorized("Invalid credentials");
        //}


        //[HttpPost("old2_login")]
        //public async Task<IActionResult> old2_login(LoginDto dto)
        //{
        //    var user = await _context.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .ThenInclude(r => r.RolePermissions)
        //        .ThenInclude(rp => rp.Permission)
        //        .FirstOrDefaultAsync(u => u.Username == dto.Username);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //    {
        //        return Unauthorized("Invalid credentials.");
        //    }

        //    // Get roles
        //    var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        //    // Get permissions
        //    var permissions = user.UserRoles
        //        .SelectMany(ur => ur.Role.RolePermissions)
        //        .Select(rp => rp.Permission.Name)
        //    .Distinct()
        //        .ToList();

        //    var token = _jwtService.GenerateToken(user, roles, permissions);

        //    // Create refresh token
        //    var refreshToken = new RefreshToken
        //    {
        //        Id = Guid.NewGuid(),
        //        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        IsRevoked = false,
        //        UserId = user.Id
        //    };

        //    //get oldTokens Before saving new refresh token
        //    var oldTokens = await _context.RefreshTokens
        //        .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
        //        .ToListAsync();

        //    //revoke the previous oldTokens
        //    foreach (var old in oldTokens)
        //        old.IsRevoked = true;

        //    _context.RefreshTokens.Add(refreshToken);
        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        token,
        //        refreshToken = refreshToken.Token
        //    });
        //}



        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterDto dto)
        //{
        //    if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        //        return BadRequest("Username already exists");

        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        //    var user = new User
        //    {
        //        Username = dto.Username,
        //        PasswordHash = hashedPassword,
        //        FullName = dto.FullName,
        //        Email = dto.Email,
        //        Phonenumber = dto.Phonenumber,
        //        Address = dto.Address,
        //        Description = dto.Description,
        //        CreatedById = dto.CreatedById,
        //        CreationDate = DateTime.Now
        //    };

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    //return CreatedAtAction(nameof(GetRoles), new { id = user.Id }, role);

        //    return Ok("User registered successfully");
        //}
    }
}
//[HttpPost("old1_refresh-token")]
//public async Task<IActionResult> old1_RefreshToken([FromBody] string refreshToken)
//{
//    var storedToken = await _context.RefreshTokens
//    .Include(rt => rt.User)
//    .ThenInclude(u => u.UserRoles)
//    .ThenInclude(ur => ur.Role)
//    .ThenInclude(r => r.RolePermissions)
//    .ThenInclude(rp => rp.Permission)
//    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

//    if (storedToken == null)
//        return Unauthorized("Invalid or expired refresh token.");

//    var user = storedToken.User;

//    // Re-evaluate roles and permissions from DB
//    var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

//    var permissions = user.UserRoles
//        .SelectMany(ur => ur.Role.RolePermissions)
//        .Select(rp => rp.Permission.Name)
//        .Distinct()
//        .ToList();

//    // Generate new token
//    var newToken = _jwtService.GenerateToken(user, roles, permissions);

//    // Create new refresh token
//    var newTokenEntry = new RefreshToken
//    {
//        Id = Guid.NewGuid(),
//        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
//        Expires = DateTime.UtcNow.AddDays(7),
//        IsRevoked = false,
//        UserId = user.Id
//    };

//    // Revoke old token
//    storedToken.IsRevoked = true;

//    _context.RefreshTokens.Add(newTokenEntry);
//    await _context.SaveChangesAsync();

//    return Ok(new
//    {
//        token = newToken,
//        refreshToken = newTokenEntry.Token

//    });
//}
