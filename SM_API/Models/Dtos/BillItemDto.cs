namespace SM_API.Models.Dtos
{
    public class BillItemDto
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
