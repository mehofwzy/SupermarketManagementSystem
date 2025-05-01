using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SM_Web.Services
{
    public class JwtHelperService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public JwtHelperService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private string? GetToken()
        {
            var context = _contextAccessor.HttpContext;
            var token = context?.Request.Cookies["jwt"];
            return token;
        }

        public Guid GetCurrentUserId1()
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.Parse(id);
        }
        public string? GetCurrentUserId2()
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return id;
        }
        public string? GetUsername()
        {
            var token = GetToken();
            if (token == null) return null;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        public List<string> GetRoles()
        {
            var token = GetToken();
            if (token == null) return [];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        }

        public List<string> GetPermissions()
        {
            var token = GetToken();
            if (token == null) return [];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        }

        public bool HasPermission(string permission)
        {
            var tata = GetPermissions().Where(co=>co.Contains("")).ToList().Count;


            return GetPermissions().Contains(permission);
        }


    }
}
