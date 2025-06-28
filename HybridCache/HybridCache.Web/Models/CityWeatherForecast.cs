namespace HybridCache.Web.Models;

public sealed record CityWeatherForecast(string CityName, DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);
