var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                    .WithDataVolume()
                   .WithRedisInsight();

var catalogApi = builder.AddProject<Projects.catalog>("catalogservice")
                      .WithExternalHttpEndpoints();

var basketApi = builder.AddProject<Projects.Basket>("BasketService")
                        .WithReference(redis)
                        .WithExternalHttpEndpoints();

builder.Build().Run();
