using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }


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

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
