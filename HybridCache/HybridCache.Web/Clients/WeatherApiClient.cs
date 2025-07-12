using System.Diagnostics.CodeAnalysis;
using HybridCache.ApiModels;
using HybridCache.Web.Models;
using Microsoft.Extensions.Caching.Hybrid;
using Cache = Microsoft.Extensions.Caching.Hybrid.HybridCache;

namespace HybridCache.Web.Clients;

[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated via dependency injection")]
internal sealed class WeatherApiClient(HttpClient httpClient, Cache cache)
{
    private readonly HybridCacheEntryOptions _readOnlyCacheEntryOptions
        = new() { Flags = HybridCacheEntryFlags.DisableUnderlyingData };

    internal bool IsCacheEnabled { get; set; }

    internal bool PopulateCacheOnMiss { get; set; }

    internal async Task<CityWeatherForecast[]> GetWeatherForecastsAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default)
    {
        List<WeatherForecast> forecasts = [];

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weather-forecast", cancellationToken).ConfigureAwait(false))
        {
            if (forecasts.Count >= maxItems)
            {
                break;
            }

            if (forecast is not null)
            {
                forecasts.Add(forecast);
            }
        }

        var cityForecasts = await Task.WhenAll(
            forecasts.Select(async forecast =>
            {
                var city = await GetCityAsync(forecast.CityId, cancellationToken)
                    .ConfigureAwait(false);

                return new CityWeatherForecast(
                    city.Name,
                    forecast.Date,
                    forecast.TemperatureC,
                    forecast.TemperatureF,
                    forecast.Summary);
            }))
            .ConfigureAwait(false);

        return cityForecasts;
    }

    private async Task<City> GetCityAsync(long id, CancellationToken cancellationToken)
    {
        if (IsCacheEnabled)
        {
            var cachedCity = await cache
                .GetOrCreateAsync(
                    $"/cities/{id}",
                    async ct => await GetCityFromApiAsync(id, ct).ConfigureAwait(false),
                    PopulateCacheOnMiss ? null : _readOnlyCacheEntryOptions,
                    ["City"],
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (cachedCity is not null)
            {
                return cachedCity;
            }
        }

        return await GetCityFromApiAsync(id, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<City> GetCityFromApiAsync(long id, CancellationToken cancellationToken)
        => await httpClient.GetFromJsonAsync<City>($"/cities/{id}", cancellationToken)
            .ConfigureAwait(false)
            ?? new City(id, "Unknown");
}
