using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SM_API.Data;
using SM_API.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using SM_API.Authorization;
using SM_API.Filters;
using Microsoft.AspNetCore.Identity;
using SM_API.Models;



var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("https://localhost:5001");

builder.Services.AddDbContext<SupermarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JwtService
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddAuthorization(options =>
{
    // Nothing needed here unless you're adding named policies
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizePermissionFilter>();
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SM_API", Version = "v1" });

    // Enable JWT auth in Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



// Add JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ClockSkew = TimeSpan.Zero // 🔥 Disable the default 5-minute skew
        };
    });




var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
