using System.Text.Json.Serialization;

namespace SM_API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        [JsonIgnore]

        public ProductCategory Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

}
