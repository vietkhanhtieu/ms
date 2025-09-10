using catalog.IntegrationEvents;
using catalog.Models;
using catalog.Repository.Implementations;
using catalog.Repository.Interfaces;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using IntegrationEvenlogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDbContext")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICatalogRepository ,CatalogRepository>();
builder.Services.AddScoped<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

builder.Services.AddTransient<IIntegrationEventlogService, IntegrationEventLogServices<CatalogContext>>();
//builder.Services.AddTransient<IEventBus, RabbitMQEventBus>();


var app = builder.Build();

// Apply pending EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<CatalogContext>();
	db.Database.Migrate();
}

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    db.Database.Migrate();
}
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
