using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.API.Services.Interfaces;
using StockExchangeSystem.API.Services;
using StockExchangeSystem.Data;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.API.Utilities;
using StockExchangeSystem.API.Middleware;
using StockExchangeSystem.Data.Repositories;
using Microsoft.OpenApi.Models;
using StockExchangeSystem.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IStockRepository, StockRepository>();

builder.Services.AddScoped<ITradeRepository, TradeRepository>();

builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();

builder.Services.AddScoped<ITradeGeneratorService, TradeGeneratorService>();

builder.Services.AddScoped<IPaginationHelper, PaginationHelper>();

// Database Configuration with Dependency Injection
builder.Services.AddDbContext<StockExchangeDbContext>(options =>
{
    var databaseConfiguration = (IDatabaseConfiguration)(builder.Configuration.GetValue<bool>("UseInMemoryDatabase")
        ? new InMemoryDatabaseConfiguration()
        : new TestingDatabaseConfiguration());

    options.UseSqlite(databaseConfiguration.GetConnectionString());
});

// Conditional IDatabaseConfiguration registration
if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    builder.Services.AddScoped<IDatabaseConfiguration, InMemoryDatabaseConfiguration>();
}
else
{
    builder.Services.AddScoped<IDatabaseConfiguration, TestingDatabaseConfiguration>();
}

// Register the Swagger generator, defining one or more Swagger documents
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock Exchange System API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Exchange System API V1");
    c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CustomExceptionMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
