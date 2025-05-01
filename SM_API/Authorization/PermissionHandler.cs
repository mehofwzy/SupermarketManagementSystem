using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SM_API.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var hasPermission = context.User.Claims
                .Any(c => c.Type == "permission" && c.Value == requirement.RequiredPermission);

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

}
