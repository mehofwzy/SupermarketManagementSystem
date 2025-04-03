using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class BillItem
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        [JsonIgnore]

        public Bill Bill { get; set; }
        public Guid ProductId { get; set; }
        [JsonIgnore]

        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
