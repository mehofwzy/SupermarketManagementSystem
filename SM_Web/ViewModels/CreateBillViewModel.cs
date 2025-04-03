using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM_Web.ViewModels
{
    public class CreateBillViewModel
    {
        public Guid? ClientId { get; set; } // Selected ClientId
        public string? ClientName { get; set; } 
        public string? ClientPhone { get; set; } 
        public string? ClientAddress { get; set; }
        public Guid BillTypeId { get; set; } // Selected BillTypeId
        public DateTime BillDate { get; set; } = DateTime.Now;

        // These should NOT be required during form submission
        public List<Models.Client> Clients { get; set; } = new();
        public List<Models.Product> Products { get; set; } = new();
        public List<Models.BillType> BillTypes { get; set; } = new();
        public List<BillItemViewModel> Items { get; set; } = new List<BillItemViewModel>();
    }
}