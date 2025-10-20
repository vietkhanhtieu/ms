using catalog;
using catalog.Contractor;
using catalog.IntegrationEvents;
using catalog.JobExcutor;
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
//builder.Services.AddDbContext<CatalogContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDbContext")));

builder.AddNpgsqlDbContext<CatalogContext>(connectionName: "CatalogDbContext");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICatalogRepository ,CatalogRepository>();
builder.Services.AddScoped<IJobRepository ,JobRepository>();
builder.Services.AddScoped<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
builder.Services.AddScoped<IIntergrationEventLogRepository, IntergrationEventLogRepository>();

builder.Services.AddScoped<IIntegrationEventlogService, IntegrationEventLogServices<CatalogContext>>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
builder.Services.AddScoped<IJobFactory, JobFactory>();
builder.Services.AddHostedService<WorkerRole>();

// backgroug task
builder.Services.AddTransient<SyncStockExecutor>();
builder.Services.AddTransient<EventExecutor>();

builder.Services.AddTransient<SyncStockJob>();
builder.Services.AddTransient<EventJob>();
builder.Services.AddTransient<JobExecutorFactory>();



var app = builder.Build();

// Apply pending EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    //var db = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    //db.Database.Migrate();
    var repo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
    await repo.InitTask();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
