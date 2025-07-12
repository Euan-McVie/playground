var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight();

var apiService = builder.AddProject<Projects.HybridCache_ApiService>("api-service")
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.HybridCache_Web>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync()
    .ConfigureAwait(false);
