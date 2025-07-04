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
    internal bool IsCacheEnabled { get; set; }

    internal async Task<City[]> GetCitiesAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<City[]>($"/cities/", cancellationToken)
            .ConfigureAwait(false)
            ?? [];

    internal async Task<City> UpdateCityAsync(City city, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.PutAsJsonAsync($"/cities/{city.Id}", city, cancellationToken)
            .ConfigureAwait(false);

        return result switch
        {
            var response when response.IsSuccessStatusCode
                => await response.Content.ReadFromJsonAsync<City>(cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to read city from response."),
            var response => throw new InvalidOperationException($"Failed to update city: {response.ReasonPhrase}"),
        };
    }

    internal async Task<(CityWeatherForecast[] Forecasts, DateTimeOffset LastUpdated)> GetWeatherForecastsAsync(
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

        return (cityForecasts, DateTimeOffset.Now);
    }

    private async Task<City> GetCityAsync(long id, CancellationToken cancellationToken)
    {
        if (IsCacheEnabled)
        {
            var cachedCity = await cache
                .GetOrCreateAsync<City>(
                    $"/cities/{id}",
                    null!,
                    new HybridCacheEntryOptions { Flags = HybridCacheEntryFlags.DisableUnderlyingData },
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
