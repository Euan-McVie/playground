namespace HybridCache.ApiModels;

public sealed record WeatherForecast(long CityId, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
