using System;

namespace SM_Web.ViewModels
{
    public class ClientViewModel
    {
        public List<Models.Client>? Clients { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }

        // Filtering Fields
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
