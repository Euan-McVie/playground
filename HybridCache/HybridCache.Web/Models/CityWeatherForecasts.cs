namespace HybridCache.Web.Models;

public sealed record CityWeatherForecasts(IReadOnlyList<CityWeatherForecast> Forecasts)
{
    public DateTimeOffset CityLocalCachedAt { get; } = Forecasts.Max(f => f.City.LocalCachedAt);

    public DateTimeOffset CityDistributedCachedAt { get; } = Forecasts.Max(f => f.City.DistributedCachedAt);
}
