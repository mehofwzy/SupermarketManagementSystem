using System;
using System.Collections.Generic;

namespace SM_Web.ViewModels
{
    public class BillViewModel
    {
        public List<Models.Bill>? Bills { get; set; }
        public List<Models.Client>? Clients { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }

        // Filtering Fields
        public Guid? ClientId { get; set; }
        public string? ClientPhone { get; set; }
        public int? BillNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? BillTypeId { get; set; }
    }
}
