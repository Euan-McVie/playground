using HybridCache.ApiModels;
using HybridCache.Web.Models;

namespace HybridCache.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<(CityWeatherForecast[] Forecasts, DateTimeOffset LastUpdated)> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<CityWeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weather-forecast", cancellationToken).ConfigureAwait(false))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }

            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(new CityWeatherForecast(
                    CityName: "Unknown",
                    Date: forecast.Date,
                    TemperatureC: forecast.TemperatureC,
                    TemperatureF: forecast.TemperatureF,
                    Summary: forecast.Summary));
            }
        }

        return (forecasts?.ToArray() ?? [], DateTimeOffset.Now);
    }
}
