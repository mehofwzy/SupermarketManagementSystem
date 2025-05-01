using Microsoft.AspNetCore.Authorization;

namespace SM_API.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public string Permission { get; }

        public AuthorizePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }

}
