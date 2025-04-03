namespace SM_API.Models.Dtos
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? Balance { get; set; }
    }
}
