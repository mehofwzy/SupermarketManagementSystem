using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SM_API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? FisrtName { get; set; }
        public string? LastName { get; set; }
        public string? Phonenumber { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public DateTime? LastLogin { get; set; }

        [JsonIgnore]
        public User CreatedBy { get; set; }
        
        [JsonIgnore]
        public User? ModifiedBy { get; set; }
        
        [JsonIgnore]
        public ICollection<UserRole> UserRoles { get; set; }
        
        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
