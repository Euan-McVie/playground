using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace HybridCache.ServiceDefaults;

public static class FusionCacheX
{
    public static void AddFusionCache(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddRedisDistributedCache("cache");

        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(options => options
                .SetDuration(TimeSpan.FromSeconds(10))
                .SetDistributedCacheDuration(TimeSpan.FromSeconds(30)))
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithRegisteredDistributedCache()
            .WithStackExchangeRedisBackplane(options => options.Configuration = builder.Configuration.GetConnectionString("cache"))
            .AsHybridCache();

        builder.Services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.AddFusionCacheInstrumentation(o =>
                {
                    o.IncludeMemoryLevel = true;
                });
            }).WithMetrics(builder =>
            {
                builder.AddFusionCacheInstrumentation(o =>
                {
                    o.IncludeMemoryLevel = true;
                    o.IncludeDistributedLevel = true;
                    o.IncludeBackplane = true;
                });
            });
    }
}
