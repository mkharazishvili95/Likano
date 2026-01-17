using Likano.Application.Configuration;
using Likano.Application.Features.Auth.Commands.Register;
using Likano.Application.Features.Category.Queries.Get;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Interfaces;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Queries.Product;
using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Repositories;
using Likano.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<GetCategoryHandler>());
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<ChangeProductStatusHandler>());
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<GetAllProductsForSearchHandler>());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IManageRepository, ManageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductQueries, ProductQueries>();

builder.Services.AddScoped<RegisterUserValidator>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.Configure<ImageKitSettings>(
    builder.Configuration.GetSection("ImageKit")
);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

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
