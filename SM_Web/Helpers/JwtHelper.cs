using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SM_Web.Helpers
{
    public static class JwtHelper
    {
        public static string? GetUserIdFromToken(HttpContext? context, string token)
        {
            //var token = HttpContextAccessor.HttpContext?.Request.Cookies["jwt"];
            
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var UserId = jwt?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return UserId;
        } 
        public static string? GetUsernameFromToken(string token)
        {
            //var token = HttpContextAccessor.HttpContext?.Request.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var Username = jwt?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            return Username;
        }
        public static List<string>? GetUserRoleClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);


            var Roles = jwt?.Claims
                      .Where(c => c.Type == ClaimTypes.Role)
                      .Select(c => c.Value)
                      .ToList();


            return Roles;
        }

        public static List<string>? GetUserPermissionClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var permissions = jwt?.Claims
                      .Where(c => c.Type == "permission")
                      .Select(c => c.Value)
                      .ToList();

            return permissions;
        }
    }
}