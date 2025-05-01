namespace SM_API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ReplacedByToken { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow <= Expires;
    }
}
