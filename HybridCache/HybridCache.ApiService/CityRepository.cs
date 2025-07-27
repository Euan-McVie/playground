using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using HybridCache.ApiModels;

namespace HybridCache.ApiService;

[SuppressMessage("Design", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI")]
internal sealed class CityRepository
{
    private ConcurrentDictionary<long, City> Cities { get; }
        = new()
        {
            [1] = new City(1, "Edinburgh"),
            [2] = new City(2, "Glasgow"),
            [3] = new City(3, "Aberdeen"),
        };

    internal IReadOnlySet<City> GetCities() => Cities.Values.ToFrozenSet();

    internal City? GetCity(long id)
    {
        if (Cities.TryGetValue(id, out var city))
        {
            return city;
        }

        return null;
    }

    internal City? UpdateCity(City city, City existingCity)
    {
        if (!Cities.TryUpdate(city.Id, city, existingCity))
        {
            return null;
        }

        return city;
    }
}
