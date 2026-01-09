using Likano.Application.Features.Auth.Commands.Register;
using Likano.Application.Features.Category.Queries.Get;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Interfaces;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IManageRepository, ManageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<RegisterUserValidator>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
