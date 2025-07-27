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
        endpoints.MapGet("/cities/", (CityRepository cityRepository) =>
        {
            var cities = cityRepository.GetCities();
            return Results.Ok(cities);
        })
        .WithName("GetCities");
    }

    private static void MapGetCity(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/cities/{id}", async (
            [FromRoute] long id,
            Cache cache,
            CityRepository cityRepository,
            [FromQuery(Name = "useCache")] bool useCache = false) =>
        {
            // Simulate a delay to mimic slower network.
            await Task.Delay(1000).ConfigureAwait(false);

            if (useCache)
            {
                var cachedCity = await cache
                    .GetOrCreateAsync(
                        $"/cities/{id}",
                        _ =>
                        {
                            var city = cityRepository.GetCity(id);
                            return ValueTask.FromResult(
                                city is not null
                                ? new CachedCity(id, city.Name)
                                : new CachedCity(id, "Unknown"));
                        },
                        tags: ["City"])
                    .ConfigureAwait(false);

                if (cachedCity is not null)
                {
                    return Results.Ok(new City(cachedCity.Id, cachedCity.Name));
                }
            }

            var city = cityRepository.GetCity(id);

            if (city is not null)
            {
                // Simulate a delay to mimic a real-world scenario.
                await Task.Delay(2000).ConfigureAwait(false);
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
            CityRepository cityRepository,
            [FromQuery(Name = "clearCache")] bool clearCache = false) =>
        {
            if (id != city.Id)
            {
                return Results.BadRequest("City Id in the URL does not match the Id in the request body.");
            }

            var existingCity = cityRepository.GetCity(id);
            if (existingCity is null)
            {
                return Results.NotFound($"City with Id {id} not found.");
            }

            var updatedCity = cityRepository.UpdateCity(city, existingCity);
            if (updatedCity is null)
            {
                return Results.Conflict($"City with Id {id} was modified by another request.");
            }

            if (clearCache)
            {
                await cache.RemoveByTagAsync("City")
                    .ConfigureAwait(false);
            }

            return Results.Ok(updatedCity);
        })
        .WithName("PutCity");
    }
}
