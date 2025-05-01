using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SM_API.Services;
using System.Security.Claims;

namespace SM_API.Attributes
{
    public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _objectName;
        private readonly string _permissionName;

        public HasPermissionAttribute(string objectName, string permissionName)
        {
            _objectName = objectName;
            _permissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var permissionService = httpContext.RequestServices.GetRequiredService<IPermissionService>();

            var hasPermission = await permissionService.UserHasPermissionAsync(userId, _objectName, _permissionName);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
