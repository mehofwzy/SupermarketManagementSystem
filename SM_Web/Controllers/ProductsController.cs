using Microsoft.AspNetCore.Mvc;
using SM_Web.Models;
using SM_Web.Services;
using SM_Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SM_Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApiService _apiService;

        public ProductsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? code, string? name, Guid? categoryId, decimal? price, int? stockQuantity, int page = 1, int pageSize = 5)
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "code", code },
                { "name", name },
                { "categoryId", categoryId?.ToString() },
                { "price", price?.ToString() },
                { "stockQuantity", stockQuantity?.ToString() },
                { "page", page.ToString() },
                { "pageSize", pageSize.ToString() }
            };

            var response = await _apiService.GetAsync<ProductViewModel>("products", queryParams);


            var model = new ProductViewModel
            {
                Products = response.Products,
                TotalRecords = response.TotalRecords,
                Page = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                Code= code,
                Name= name,
                Price = price,
                StockQuantity = stockQuantity
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _apiService.GetAsync<Product>($"products/{id}");
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);

            var createdProduct = await _apiService.PostAsync<Product, Product>("products", product);

            if (createdProduct == null)
            {
                ModelState.AddModelError("", "Failed to create Product.");
                return View(product);
            }

            return RedirectToAction("Details", new { id = createdProduct.Id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _apiService.GetAsync<Product>($"products/{id}");
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            if (!ModelState.IsValid) return View(product);

            var result = await _apiService.PutAsync($"products/{id}", product);
            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error updating product.");
            return View(product);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _apiService.GetAsync<Product>($"products/{id}");
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var result = await _apiService.DeleteAsync($"products/{id}");
            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error deleting product.");
            return View();
        }
    }
}
