namespace SM_API.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string objectName, string permissionName);
    }
}
