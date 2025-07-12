using System.Diagnostics.CodeAnalysis;
using HybridCache.ApiModels;

namespace HybridCache.Web.Clients;

[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated via dependency injection")]
internal sealed class CitiesApiClient(HttpClient httpClient)
{
    internal bool ClearCache { get; set; }

    internal async Task<City[]> GetCitiesAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<City[]>($"/cities/", cancellationToken)
            .ConfigureAwait(false)
            ?? [];

    internal async Task<City> UpdateCityAsync(City city, CancellationToken cancellationToken = default)
    {
        string uri = $"/cities/{city.Id}";

        if (ClearCache)
        {
            uri += "?clearCache=true";
        }

        var result = await httpClient.PutAsJsonAsync(uri, city, cancellationToken)
            .ConfigureAwait(false);

        return result switch
        {
            var response when response.IsSuccessStatusCode
                => await response.Content.ReadFromJsonAsync<City>(cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to read city from response."),
            var response => throw new InvalidOperationException($"Failed to update city: {response.ReasonPhrase}"),
        };
    }
}
