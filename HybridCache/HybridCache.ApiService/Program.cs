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
City[] cities = [new City(1, "Edinburgh"), new City(2, "Glasgow"), new City(3, "Aberdeen")];

app.MapGet("/weather-forecast", async () =>
{
    // Simulate a delay to mimic a real-world scenario.
    await Task.Delay(1000).ConfigureAwait(false);

    var forecasts = cities
        .SelectMany(city => Enumerable
            .Range(1, 3)
            .Select(index =>
                new WeatherForecast(
                    city.Id,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    RandomNumberGenerator.GetInt32(-20, 55),
                    summaries[RandomNumberGenerator.GetInt32(summaries.Length)])))
        .ToArray();

    return forecasts;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

await app.RunAsync()
    .ConfigureAwait(false);
