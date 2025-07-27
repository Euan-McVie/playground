using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using HybridCache.ApiModels;

namespace HybridCache.ApiService;

[SuppressMessage("Design", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI")]
internal sealed class WeatherForecastRepository
{
    internal string[] WeatherSummaries { get; }
        = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
}
