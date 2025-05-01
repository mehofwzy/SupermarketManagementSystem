using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Net.Http.Headers;
using SM_Web.ViewModels;
using System.Text.Json;
using System.Text;
using SM_Web.Controllers;
using SM_Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SM_Web.Services
{
    public class ApiService

    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = accessor;

        }




        // Updated GetAsync to support query parameters
        //public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string?>? queryParams = null)
        //{
        //    var url = $"api/{endpoint}";

        //    if (queryParams != null && queryParams.Count > 0)
        //    {
        //        var queryString = string.Join("&", queryParams
        //            .Where(kv => !string.IsNullOrEmpty(kv.Value)) // Exclude null or empty values
        //            .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

        //        url += $"?{queryString}";
        //    }

        //    await AddAuthHeaderAsync();


        //    var response = await _httpClient.GetFromJsonAsync<T>(url);


        //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //    {
        //        if (await RefreshTokenAsync())
        //        {
        //            await AddAuthHeaderAsync(); // Refresh headers
        //            response = await _httpClient.GetFromJsonAsync<T>(url); // Retry
        //        }
        //    }

        //    return response;
        //}


        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string?>? queryParams = null)
        {
            var url = $"api/{endpoint}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

                url += $"?{queryString}";
            }

            await AddAuthHeaderAsync();

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshTokenAsync())
                {
                    await AddAuthHeaderAsync(); // Refresh headers
                    response = await _httpClient.GetAsync(url); // Retry
                }
            }

            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }





        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            await AddAuthHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync($"api/{endpoint}", data);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshTokenAsync())
                {
                    await AddAuthHeaderAsync(); // Refresh headers
                    response = await _httpClient.PostAsJsonAsync($"api/{endpoint}", data); // Retry
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {errorMessage}");
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            await AddAuthHeaderAsync();

            var response = await _httpClient.PutAsJsonAsync($"api/{endpoint}", data);


            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshTokenAsync())
                {
                    await AddAuthHeaderAsync(); // Refresh headers
                    response = await _httpClient.PutAsJsonAsync($"api/{endpoint}", data); // Retry
                }
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {

            await AddAuthHeaderAsync();

            var response = await _httpClient.DeleteAsync($"api/{endpoint}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await RefreshTokenAsync())
                {
                    await AddAuthHeaderAsync(); // Refresh headers
                    response = await _httpClient.DeleteAsync($"api/{endpoint}"); // Retry
                }
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<TokenResponse?> LoginAsync(LoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            return result;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var content = new StringContent(
                JsonSerializer.Serialize(new { RefreshToken = refreshToken }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("api/auth/refreshToken", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            if (result == null || string.IsNullOrEmpty(result.Token))
                return false;

            // Update cookies
            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            return true;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
