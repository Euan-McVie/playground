using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using HybridCache.ApiModels;

namespace HybridCache.ApiService;

internal static class Repository
{
    internal static string[] WeatherSummaries { get; }
        = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    internal static ConcurrentDictionary<long, City> Cities { get; }
        = new()
        {
            [1] = new City(1, "Edinburgh"),
            [2] = new City(2, "Glasgow"),
            [3] = new City(3, "Aberdeen"),
        };
}
