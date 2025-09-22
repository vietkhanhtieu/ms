using Microsoft.Extensions.Logging;
using ScheduleJob.Model;
using ScheduleJob;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ScheduleJob.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddDbContext<ScheduleDbContext>(options =>
            options.UseNpgsql("Server=localhost;Port=5432;Database=ScheduleDb;User Id=postgres;Password=postgre"));

        //builder.Services.AddDbContext<ScheduleDbContext>(options =>
        //    options.UseNpgsql(builder.Configuration.GetConnectionString("ScheduleDbContext")));

        builder.Services.AddScoped<WorkerRole>();
        builder.Services.AddScoped<ITaskScheduleRepository, TaskScheduleRepository>();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Start role");

        var role = sp.GetRequiredService<WorkerRole>();
        await role.InitTask();
        await role.RunAsync();
    }

}