namespace SM_Web.ViewModels
{
    public class BillItemViewModel
    {
        public Guid ProductId { get; set; } // Required for API submission
        public string ProductName { get; set; } // Required for display
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
