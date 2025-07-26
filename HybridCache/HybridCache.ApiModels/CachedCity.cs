namespace HybridCache.ApiModels;

public sealed record CachedCity(long Id, string Name)
{
    public DateTimeOffset LocalCachedAt { get; } = DateTimeOffset.UtcNow;

    public DateTimeOffset DistributedCachedAt { get; init; } = DateTimeOffset.UtcNow;
}
