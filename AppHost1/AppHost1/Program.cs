var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                    .WithDataVolume()
                   .WithRedisInsight();

var scheduleDbConnectionString = builder.AddConnectionString("ScheduleDbContext");
var catalogDbConn = builder.AddConnectionString("CatalogDbContext");

var catalogApi = builder.AddProject<Projects.catalog>("catalogservice")
                      .WithReference(catalogDbConn)
                      .WithExternalHttpEndpoints();

var basketApi = builder.AddProject<Projects.Basket>("BasketService")
                        .WithReference(redis)
                        .WithExternalHttpEndpoints();


var scheduleJob = builder.AddProject<Projects.ScheduleJob>("scheduleJob")
                      .WithExternalHttpEndpoints();

builder.AddProject<Projects.Customer>("customer");

builder.Build().Run();
