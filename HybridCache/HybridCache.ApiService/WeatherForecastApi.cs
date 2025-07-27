using System.Security.Cryptography;
using HybridCache.ApiModels;

namespace HybridCache.ApiService;

internal static class WeatherForecastApi
{
    internal static IEndpointRouteBuilder MapWeatherForecastApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetWeatherForecast();

        return endpoints;
    }

    private static void MapGetWeatherForecast(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weather-forecast", async (
            WeatherForecastRepository weatherRepository,
            CityRepository cityRepository) =>
        {
            // Simulate a delay to mimic a real-world scenario.
            await Task.Delay(500).ConfigureAwait(false);

            var forecasts = cityRepository.GetCities()
                .SelectMany(city => Enumerable
                    .Range(1, 3)
                    .Select(index =>
                        new WeatherForecast(
                            city.Id,
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            RandomNumberGenerator.GetInt32(-20, 55),
                            weatherRepository.WeatherSummaries[RandomNumberGenerator.GetInt32(weatherRepository.WeatherSummaries.Length)])))
                .ToArray();

            return forecasts;
        })
        .WithName("GetWeatherForecast");
    }
}
