using ElectroKart_Api.Data;
using ElectroKart_Api.Middleware;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using ElectroKart_Api.Repositories.Auth;
using ElectroKart_Api.Repositories.Cart;
using ElectroKart_Api.Repositories.Wishlist;
using ElectroKart_Api.Services.Auth;
using ElectroKart_Api.Services.CartServices;
using ElectroKart_Api.Services.Payment;     // ? Add Payment Service
using ElectroKart_Api.Services.Products;
using ElectroKart_Api.Services.Wishlist;
using ElectroKart_Api.Settings;              // ? Add Razorpay Settings
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// DATABASE CONFIGURATION
// ---------------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------------------------------------------------------------
// CONTROLLERS
// ---------------------------------------------------------------------
builder.Services.AddControllers();

// ---------------------------------------------------------------------
// CONFIGURE RAZORPAY SETTINGS
// ---------------------------------------------------------------------
builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("RazorpaySettings")
);

// ---------------------------------------------------------------------
// REPOSITORY REGISTRATION (Dependency Injection)
// ---------------------------------------------------------------------
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>(); // ? NEW

// ---------------------------------------------------------------------
// SERVICE REGISTRATION
// ---------------------------------------------------------------------
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJWTGenerator, JWTGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IPaymentService, PaymentService>(); // ? NEW

// ---------------------------------------------------------------------
// JWT AUTHENTICATION CONFIGURATION
// ---------------------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// ---------------------------------------------------------------------
// SWAGGER CONFIGURATION (for JWT support)
// ---------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {token}' to authenticate."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ---------------------------------------------------------------------
// BUILD APP
// ---------------------------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------------------------
// MIDDLEWARE CONFIGURATION
// ---------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware (Role-based Authorization)
app.UseMiddleware<RoleAuthorizationMiddleware>();

// Map Controllers
app.MapControllers();

app.Run();
