using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using SM_Web.ViewModels;

namespace SM_Web.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<bool> CheckUserPermissionAsync(HttpContext context, string objectName, string permissionName)
        {
           var his =  context.Request.Headers;

            var user = context?.User;
            var userClaims = context?.User?.Claims;
            string? userName = context?.User.Identity?.Name;
            string? userIdentifier = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string? token = context?.Request?.Cookies["jwt"];

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userId = jwt?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            var response = await _apiService.GetAsync<bool?>($"auth/checkUserPermission?userId={userIdentifier}&objectName={objectName}&permissionName={permissionName}", null);

            if (response.HasValue)
            {
                return response.Value;
            }

            return false;
        }
    }
}
