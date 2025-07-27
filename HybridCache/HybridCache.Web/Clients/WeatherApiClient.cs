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

    internal bool IsClientCacheEnabled { get; set; }

    internal bool IsServerCacheEnabled { get; set; }

    internal bool PopulateCacheOnMiss { get; set; }

    internal async Task<CityWeatherForecasts> GetWeatherForecastsAsync(
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
                    city,
                    forecast.Date,
                    forecast.TemperatureC,
                    forecast.TemperatureF,
                    forecast.Summary);
            }))
            .ConfigureAwait(false);

        return new CityWeatherForecasts(cityForecasts);
    }

    private async Task<CachedCity> GetCityAsync(long id, CancellationToken cancellationToken)
    {
        if (IsClientCacheEnabled)
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

    private async Task<CachedCity> GetCityFromApiAsync(long id, CancellationToken cancellationToken)
    {
        var city = await httpClient.GetFromJsonAsync<City>($"/cities/{id}?useCache={IsServerCacheEnabled}", cancellationToken)
                .ConfigureAwait(false);
        return city is not null
            ? new CachedCity(id, city.Name)
            : new CachedCity(id, "Unknown");
    }
}
