using Microsoft.AspNetCore.Mvc;
using SM_Web.Services;
using SM_Web.ViewModels;
using SM_Web.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SM_Web.Controllers
{
    //[Authorize(Roles = "admin")]

    public class ClientsController : Controller
    {
        private readonly ApiService _apiService;

        public ClientsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string? name, string? address, string? phoneNumber, int page = 1, int pageSize = 5)
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "name", name },
                { "address", address },
                { "phoneNumber", phoneNumber},
                { "page", page.ToString() },
                { "pageSize", pageSize.ToString() }
            };


            var response = await _apiService.GetAsync<ClientViewModel>("clients", queryParams);



            var model = new ClientViewModel
            {
                Clients = response.Clients,
                TotalRecords = response.TotalRecords,
                Page = page,
                PageSize = pageSize,
                Name = name,
                Address = address,
                PhoneNumber = phoneNumber
            };

            return View(model);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var client = await _apiService.GetAsync<Client>($"clients/{id}");
            if (client == null) return NotFound();
            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            var createdClient = await _apiService.PostAsync<Client, Client>("clients", client);

            if (createdClient == null)
            {
                ModelState.AddModelError("", "Failed to create Client.");
                return View(client);
            }

            return RedirectToAction("Details", new { id = createdClient.Id });
        }

        // GET: Clients/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = await _apiService.GetAsync<Client>($"clients/{id}");
            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Client client)
        {
            if (!ModelState.IsValid) return View(client);

            var result = await _apiService.PutAsync($"clients/{id}", client);
            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error updating client.");
            return View(client);
        }

        // GET: Clients/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = await _apiService.GetAsync<Client>($"clients/{id}");
            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _apiService.DeleteAsync($"clients/{id}");
            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error deleting client.");
            return View();
        }
    }
}
