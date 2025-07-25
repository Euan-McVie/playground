namespace HybridCache.ApiModels;

public sealed record City(long Id, string Name)
{
    public string Name { get; set; } = Name;
}
