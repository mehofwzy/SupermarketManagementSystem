using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using SM_Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("https://localhost:5000");




// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient with API Base URL from configuration
builder.Services.AddHttpClient<ApiService>(client =>
{
    var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl");
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});


// This makes [Authorize] global
//to add fo to error page
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtHelperService>();
builder.Services.AddSession();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.AccessDeniedPath = "/Home/Error"; // For 403 (Forbidden)
//    options.LoginPath = "/Home/Login";            // For 401 (Unauthorized)
//});

builder.Services.AddAuthentication("SM_Web_CookieAuth")
    .AddCookie("SM_Web_CookieAuth", options =>
    {
        options.LoginPath = "/Home/Login";            // For 401 (Unauthorized)
        options.AccessDeniedPath = "/Home/Error"; // For 403 (Forbidden)
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();


//jwt authentication
//
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//    options.TokenValidationParameters = new TokenValidationParameters
//    {

//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
//        ClockSkew = TimeSpan.Zero

//        //ValidateIssuerSigningKey = true,
//        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("$S&e#c@r$e#t&K$e@y$1@2#3$4&6@7#8$")), // Replace with actual key
//        //ValidateIssuer = false,
//        //ValidateAudience = false,
//        //ClockSkew = TimeSpan.Zero


//    };
//});


//deep seek
//
//builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = 307;
//    options.HttpsPort = 5001;
//});

//builder.Services.AddAuthentication(options => {
//    options.DefaultScheme = "JWT_OR_COOKIE";
//    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
//})
//.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options => {
//    options.ForwardDefaultSelector = context => {
//        string authorization = context.Request.Headers[HeaderNames.Authorization];
//        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//            return JwtBearerDefaults.AuthenticationScheme;

//        return CookieAuthenticationDefaults.AuthenticationScheme;
//    };
//})
//.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
//    //options.
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
//    //options.
//});


//for the jwt Cookies Request Headers Authorization
//
//app.Use(async (context, next) =>
//{
//    var token = context.Request.Cookies["jwt"];
//    if (!string.IsNullOrEmpty(token))
//    {
//        context.Request.Headers.Authorization = "Bearer " + token;
//    }

//    await next();
//});