namespace SM_Web.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? FisrtName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }

        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public Guid? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
