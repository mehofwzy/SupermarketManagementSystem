namespace SM_API.Models
{
    public class RegisterDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }

        public Guid CreatedById { get; set; }
    }
}