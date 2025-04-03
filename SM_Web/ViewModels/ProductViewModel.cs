using System;
using System.ComponentModel.DataAnnotations;

namespace SM_Web.ViewModels
{
    public class ProductViewModel
    {
        public List<Models.Product>? Products { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }

        // Filtering Fields
        public string? Code { get; set; }
        public string? Name { get; set; }

        public Guid? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
    }
}
