namespace SM_API.Models.Dtos
{
    public class RemovePermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
