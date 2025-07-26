using HybridCache.ApiModels;

namespace HybridCache.Web.Models;

public sealed record CityWeatherForecast(CachedCity City, DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);
