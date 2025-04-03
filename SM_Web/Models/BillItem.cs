namespace SM_Web.Models
{
    public class BillItem
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
