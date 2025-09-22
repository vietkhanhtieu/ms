using Basket.IntegrationEvents.Events;
using Basket.IntegrationEvents.Handlers;
using Basket.Model;
using Basket.Repository;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BasketContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BasketContext")));

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

builder.Services.AddTransient<IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>,ProductPriceChangedIntegrationEventHandler>();
builder.AddRedisDistributedCache(connectionName: "redis");


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BasketContext>();
    db.Database.Migrate();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();
await eventBus.SubscribeAsync<ProductPriceChangedIntegrationEvent,ProductPriceChangedIntegrationEventHandler>(app.Services);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
