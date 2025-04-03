namespace SM_API.Models.Dtos
{
    public class BillDto
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientAddress { get; set; }
        public DateTime? BillDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public Guid BillTypeId { get; set; }
        public string? BillTypeName { get; set; }
        public int BillNumber { get; set; }
        public int? ItemsTotal { get; set; }
        public List<BillItemDto>? Items { get; set; }
    }
}
