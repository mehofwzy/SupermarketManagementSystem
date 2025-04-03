using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class Bill
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid BillTypeId { get; set; }  // Foreign Key
        public BillType BillType { get; set; }  // Navigation Property
        public int BillNumber { get; set; }  // New field
        public List<BillItem> Items { get; set; }
    }

}
