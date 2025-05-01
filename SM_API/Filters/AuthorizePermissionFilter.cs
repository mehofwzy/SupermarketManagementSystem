using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using SM_API.Attributes;
using SM_API.Authorization;

namespace SM_API.Filters
{

    public class AuthorizePermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizePermissionFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var endpoint = context.ActionDescriptor.EndpointMetadata;
            var permissionAttributes = endpoint.OfType<AuthorizePermissionAttribute>();

            foreach (var attr in permissionAttributes)
            {
                var result = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionRequirement(attr.Permission));

                if (!result.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }

}
