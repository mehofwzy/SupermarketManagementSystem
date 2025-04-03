
namespace SM_Web.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        //[Required]
        public string Code { get; set; }

        //[Required]
        public string Name { get; set; }

        //[Required]
        public Guid CategoryId { get; set; }


        //[Required]
        //[Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        //[Required]
        //[Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

    }
}
