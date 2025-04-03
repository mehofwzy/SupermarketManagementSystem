using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM_Web.Models;
using SM_Web.Services;
using SM_Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SM_Web.Controllers
{
    public class BillsController : Controller
    {
        private readonly ApiService _apiService;

        public BillsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var bills = await _apiService.GetAsync<List<Bill>>("bills");

        //    var model = new ViewModels.BillViewModel();

        //    model.Bills = bills;

        //    return View(model);
        //}

    public async Task<IActionResult> Index(
        Guid? clientId,
        string? clientPhone,
        int? billNumber,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? billTypeId,
        int page = 1,
        int pageSize = 5)
    {
            var queryParams = new Dictionary<string, string?>
            {
                { "clientId", clientId.ToString() },
                { "clientPhone", clientPhone },
                { "billNumber", billNumber?.ToString() },
                { "fromDate", fromDate?.ToString("yyyy-MM-dd") },
                { "toDate", toDate?.ToString("yyyy-MM-dd") },
                { "billTypeId", billTypeId?.ToString() },
                { "page", page.ToString() },
                { "pageSize", pageSize.ToString() }
            };


            var pageSizequeryParams = new Dictionary<string, string?>
            {
                { "pageSize", int.MaxValue.ToString() }
            };


            var response = await _apiService.GetAsync<BillViewModel>("bills", queryParams);
            var clients = await _apiService.GetAsync<ClientViewModel>("clients", pageSizequeryParams);

            var model = new BillViewModel
            {
                Bills = response.Bills,
                Clients = clients.Clients,
                TotalRecords = response.TotalRecords,
                Page = page,
                PageSize = pageSize,
                ClientId = clientId,
                ClientPhone = clientPhone,
                BillNumber = billNumber,
                FromDate = fromDate,
                ToDate = toDate,
                BillTypeId = billTypeId
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var bill = await _apiService.GetAsync<Bill>($"bills/{id}");

            if (bill == null)
                return NotFound();

            return View(bill);
        }

        public async Task<IActionResult> Create()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "pageSize", int.MaxValue.ToString() }
            };


            var clients = await _apiService.GetAsync<ClientViewModel>("clients", queryParams);
            var products = await _apiService.GetAsync<ProductViewModel>("products", queryParams);
            //var billTypes = await _apiService.GetAsync<List<BillType>>("billtypes");

            var viewModel = new CreateBillViewModel
            {
                Clients = clients.Clients,
                Products = products.Products
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBillViewModel model)
        {

            if (!ModelState.IsValid)
            {
                string errors = "";
                // Log errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    //Console.WriteLine(error.ErrorMessage);

                    errors += error.ErrorMessage;
                }

                return View(model);
            }

            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("Items", "At least one product must be added.");
            }

            // Convert ViewModel to API Model (Bill)
            var bill = new Bill
            {
                ClientId = model.ClientId, //model.ClientId.HasValue ? model.ClientId.Value : new Guid("D3DCCE27-4A24-41C9-ACB8-08DD71C86549"),
                ClientName = model.ClientName,
                ClientAddress = model.ClientAddress,
                ClientPhone = model.ClientPhone,
                BillDate = model.BillDate,
                BillTypeId=model.BillTypeId,
                Items = model.Items.Select(item => new BillItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            //var response = await _apiService.PostAsync("bills", bill);
            //if (!response)
            //{
            //    ModelState.AddModelError("", "Failed to create bill.");
            //    return View(model);
            //}


            var createdBill = await _apiService.PostAsync<Bill, Bill>("bills", bill);

            if (createdBill == null)
            {
                ModelState.AddModelError("", "Failed to create bill.");
                return View(model);
            }


            //return RedirectToAction(nameof(Details), createdBill.Id);
            return RedirectToAction("Details", new { id = createdBill.Id });
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _apiService.GetAsync<Bill>($"bills/{id}");
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var result = await _apiService.DeleteAsync($"bills/{id}");
            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error deleting bills.");
            return View();
        }

        //[HttpGet]
        //public JsonResult FilterBills(int page = 1, int pageSize = 10, string? clientPhone = null, int? billNumber = null, string? fromDate = null, string? toDate = null, string? billTypeId = null)
        //{

        //    var query = _context.Bills.AsQueryable();

        //    if (!string.IsNullOrEmpty(clientPhone))
        //        query = query.Where(b => b.ClientPhone.Contains(clientPhone));

        //    if (billNumber.HasValue)
        //        query = query.Where(b => b.BillNumber == billNumber);

        //    if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDateValue))
        //        query = query.Where(b => b.BillDate >= fromDateValue);

        //    if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDateValue))
        //        query = query.Where(b => b.BillDate <= toDateValue);

        //    var bills = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    return Json(new
        //    {
        //        bills,
        //        totalRecords = query.Count(),
        //        page,
        //        pageSize
        //    });
        //}

    }
}
