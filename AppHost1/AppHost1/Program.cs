var builder = DistributedApplication.CreateBuilder(args);

var catalogApi = builder.AddProject<Projects.catalog>("catalogservice")
                      .WithExternalHttpEndpoints();

var basketApi = builder.AddProject<Projects.Basket>("BasketService")
                        .WithExternalHttpEndpoints();

builder.Build().Run();
