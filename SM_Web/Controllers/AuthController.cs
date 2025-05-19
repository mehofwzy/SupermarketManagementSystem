using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text;
using SM_Web.Services;
using SM_Web.ViewModels;
using SM_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace SM_Web.Controllers
{

    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiService _apiService;


        public AuthController(IHttpClientFactory httpClientFactory, ApiService apiService)
        {
            _httpClientFactory = httpClientFactory;
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Register() => View();
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedById = new Guid("B4178093-48D2-4FB4-05D9-08DD795C50E4");

            var createdUser = await _apiService.PostAsync<RegisterViewModel, RegisterViewModel>("auth/register", model);

            if (createdUser == null)
            {
                ModelState.AddModelError("", "Failed to create Product.");
                return View(model);
            }

            TempData["Success"] = "Registration successful!";

            return RedirectToAction("Details", new { id = createdUser.Id });

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiService.LoginAsync(model);

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
                return View(model);
            }

            //test to add it in Session 
            //HttpContext.Session.SetString("jwt", result.Token);

            // Set JWT Cookie
            Response.Cookies.Append("jwt", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddDays(7)
            });

            // Set Refresh Token Cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddDays(7)
            });



            //Request.Headers.Authorization = "Bearer " + result.Token;



            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "b4178093-48d2-4fb4-05d9-08dd795c50e4") ,
            };

            var claimsIdentity = new ClaimsIdentity(claims, "SM_Web_CookieAuth");

            var authProperties = new AuthenticationProperties
            {
                
                IssuedUtc = DateTimeOffset.Now,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(60),
                IsPersistent = false,
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync("SM_Web_CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);


            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            //Response.Cookies.Delete("jwt");
            //Response.Cookies.Delete("refreshToken");
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login_old(ViewModels.LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7199/api/auth/login", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Login failed");
                return View(model);
            }

            var result = JsonSerializer.Deserialize<TokenResponse>(
                await response.Content.ReadAsStringAsync()
            );

            // Store the token in a cookie
            Response.Cookies.Append("jwt", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddHours(1)
            });


            return RedirectToAction("Index", "Home");
        }
    }
}
