using System.Text.Json.Serialization;

namespace SM_API.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
