using System;
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
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithRegisteredDistributedCache()
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
