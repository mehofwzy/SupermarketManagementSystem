using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;

namespace SM_Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Updated GetAsync to support query parameters
        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string?>? queryParams = null)
        {
            var url = $"api/{endpoint}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams
                    .Where(kv => !string.IsNullOrEmpty(kv.Value)) // Exclude null or empty values
                    .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

                url += $"?{queryString}";
            }

            return await _httpClient.GetFromJsonAsync<T>(url);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/{endpoint}", data);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {errorMessage}");
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/{endpoint}", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync($"api/{endpoint}");
            return response.IsSuccessStatusCode;
        }
    }
}
