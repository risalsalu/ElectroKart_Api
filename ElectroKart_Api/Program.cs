using ElectroKart_Api.Data;
using ElectroKart_Api.Middleware;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using ElectroKart_Api.Repositories.Auth;
using ElectroKart_Api.Repositories.Cart;
using ElectroKart_Api.Repositories.GenericRepo;
using ElectroKart_Api.Repositories.Orders;
using ElectroKart_Api.Repositories.Payments;
using ElectroKart_Api.Repositories.Wishlist;
using ElectroKart_Api.Services;
using ElectroKart_Api.Services.Admin;
using ElectroKart_Api.Services.Auth;
using ElectroKart_Api.Services.CartServices;
using ElectroKart_Api.Services.Orders;
using ElectroKart_Api.Services.Payments;
using ElectroKart_Api.Services.Products;
using ElectroKart_Api.Services.Wishlist;
using ElectroKart_Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// 1. Configure Database
// ------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ------------------------------------------------------
// 2. Add Controllers
// ------------------------------------------------------
builder.Services.AddControllers();

// ------------------------------------------------------
// 3. Configure App Settings
// ------------------------------------------------------
builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("RazorpaySettings")
);
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// ------------------------------------------------------
// 4. Register Repositories
// ------------------------------------------------------
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // <-- User repo for dashboard & auth

// ------------------------------------------------------
// 5. Register Services
// ------------------------------------------------------
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJWTGenerator, JWTGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<IAdminUserService, AdminUserService>(); // <-- Admin User Management

// ------------------------------------------------------
// 6. JWT Authentication
// ------------------------------------------------------
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

// ------------------------------------------------------
// 7. Swagger / OpenAPI
// ------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElectroKart API",
        Version = "v1"
    });

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

// ------------------------------------------------------
// 8. Build App
// ------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------
// 9. Middleware Pipeline
// ------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global Error Handling
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom Role Middleware
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.MapControllers();

app.Run();
