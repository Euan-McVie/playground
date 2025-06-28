using HybridCache.ApiModels;
using HybridCache.Web.Models;

namespace HybridCache.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<(CityWeatherForecast[] Forecasts, DateTimeOffset LastUpdated)> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
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
                var city = await GetCityAsync(forecast.CityId, cancellationToken).ConfigureAwait(false);
                return new CityWeatherForecast(
                    city.Name,
                    forecast.Date,
                    forecast.TemperatureC,
                    forecast.TemperatureF,
                    forecast.Summary);
            })).ConfigureAwait(false);

        return (cityForecasts, DateTimeOffset.Now);
    }

    private async Task<City> GetCityAsync(long id, CancellationToken cancellationToken)
    {
        return await httpClient
            .GetFromJsonAsync<City>($"/cities/{id}", cancellationToken)
            .ConfigureAwait(false)
            ?? new City(id, "Unknown");
    }
}
