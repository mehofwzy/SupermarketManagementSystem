using Microsoft.AspNetCore.Authorization;

namespace SM_API.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string RequiredPermission { get; }

        public PermissionRequirement(string permission)
        {
            RequiredPermission = permission;
        }
    }

}
