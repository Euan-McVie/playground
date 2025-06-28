using System.Collections.Frozen;
using System.Security.Cryptography;
using HybridCache.ApiModels;
using HybridCache.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
var cities = new Dictionary<long, City>()
{
    { 1, new City(1, "Edinburgh") },
    { 2, new City(2, "Glasgow") },
    { 3, new City(3, "Aberdeen") },
}.ToFrozenDictionary();

app.MapGet("/weather-forecast", async () =>
{
    // Simulate a delay to mimic a real-world scenario.
    await Task.Delay(1000).ConfigureAwait(false);

    var forecasts = cities
        .SelectMany(city => Enumerable
            .Range(1, 3)
            .Select(index =>
                new WeatherForecast(
                    city.Key,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    RandomNumberGenerator.GetInt32(-20, 55),
                    summaries[RandomNumberGenerator.GetInt32(summaries.Length)])))
        .ToArray();

    return forecasts;
})
.WithName("GetWeatherForecast");

app.MapGet("/cities/{id}", async (long id) =>
{
    // Simulate a delay to mimic a real-world scenario.
    await Task.Delay(5000).ConfigureAwait(false);

    if (cities.TryGetValue(id, out var city))
    {
        return Results.Ok(city);
    }

    return Results.NotFound();
})
.WithName("GetCities");

app.MapDefaultEndpoints();

await app.RunAsync()
    .ConfigureAwait(false);
