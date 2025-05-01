using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SM_API.Attributes
{
 
    namespace SM_API.Attributes
    {
        public class JWTHasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
        {
            private readonly string _permission;

            public JWTHasPermissionAttribute(string permission)
            {
                _permission = permission;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var user = context.HttpContext.User;

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!user.Identity?.IsAuthenticated ?? false)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }

                var hasPermission = user.Claims.Any(c => c.Type == "permission" && c.Value == _permission);

                if (!hasPermission)
                {
                    context.Result = new JsonResult(new { message = $"User does not have permission: {_permission}" })
                    { StatusCode = StatusCodes.Status403Forbidden };
                }
            }
        }
    }
}
