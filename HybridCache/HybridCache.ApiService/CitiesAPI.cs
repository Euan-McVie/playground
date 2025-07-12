using HybridCache.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Cache = Microsoft.Extensions.Caching.Hybrid.HybridCache;

namespace HybridCache.ApiService;

internal static class CitiesApi
{
    internal static IEndpointRouteBuilder MapCitiesApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetCities();
        endpoints.MapGetCity();
        endpoints.MapPutCity();

        return endpoints;
    }

    private static void MapGetCities(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/cities/", () =>
        {
            var cities = Repository.Cities.Values.ToArray();
            return Results.Ok(cities);
        })
        .WithName("GetCities");
    }

    private static void MapGetCity(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/cities/{id}", async ([FromRoute] long id) =>
        {
            // Simulate a delay to mimic a real-world scenario.
            await Task.Delay(3000).ConfigureAwait(false);

            if (Repository.Cities.TryGetValue(id, out var city))
            {
                return Results.Ok(city);
            }

            return Results.NotFound();
        })
        .WithName("GetCity");
    }

    private static void MapPutCity(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/cities/{id}", async (
            [FromRoute] long id,
            [FromBody] City city,
            Cache cache,
            [FromQuery(Name = "clearCache")] bool clearCache = false) =>
        {
            if (id != city.Id)
            {
                return Results.BadRequest("City Id in the URL does not match the Id in the request body.");
            }

            if (!Repository.Cities.TryGetValue(id, out var existingCity))
            {
                return Results.NotFound($"City with Id {id} not found.");
            }

            if (!Repository.Cities.TryUpdate(id, city, existingCity))
            {
                return Results.Conflict($"City with Id {id} was modified by another request.");
            }

            if (clearCache)
            {
                await cache.RemoveByTagAsync("City")
                    .ConfigureAwait(false);
            }

            return Results.Ok(city);
        })
        .WithName("PutCity");
    }
}
