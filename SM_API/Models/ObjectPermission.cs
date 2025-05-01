using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class ObjectPermission
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid PermissionId { get; set; }
        public bool? FullAccess { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }


        [JsonIgnore]
        public User CreatedBy { get; set; }

        [JsonIgnore]
        public User? ModifiedBy { get; set; }


        public Permission Permission { get; set; }
        public Object Object { get; set; }

        public ICollection<RoleObjectPermission> RoleObjectPermissions { get; set; }
    }
}
