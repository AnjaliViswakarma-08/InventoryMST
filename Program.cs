using FluentValidation;
using InventoryMS.Data;
using InventoryMS.Helpers;
using InventoryMS.Mappings;
using InventoryMS.Middleware;
using InventoryMS.Repositories;
using InventoryMS.Repositories.Interfaces;
using InventoryMS.Services;
using InventoryMS.Services.Interfaces;
using InventoryMS.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers with Validation Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<InventoryMS.Middleware.ValidationFilter>();
});
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(EntityToDtoProfile), typeof(DtoToEntityProfile));

// JWT Configurations
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Configure AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReportingRepository, ReportingRepository>();

// DI Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<InventoryMS.Models.User>, PasswordHasher<InventoryMS.Models.User>>();

// Auth Configurations
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"] ?? "InventoryMS_SUPER_SECRET_KEY_CHANGE_ME_1234567890";
var issuer = jwtSection["Issuer"] ?? "InventoryMS";
var audience = jwtSection["Audience"] ?? "InventoryMS";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Swagger configs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory Management System API",
        Version = "v1",
        Description = "Single-Tenant Inventory Management System API with CRUD and Auth."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token in the format: Bearer {token}"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Global Exception handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

Console.WriteLine("Initializing the database...");
await DbInitializer.InitializeAsync(app.Services);

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
