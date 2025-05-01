using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class RoleObjectPermission
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Guid ObjectPermissionId { get; set; }

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



        public Role Role { get; set; }

        public ObjectPermission ObjectPermission { get; set; }
    }
}
