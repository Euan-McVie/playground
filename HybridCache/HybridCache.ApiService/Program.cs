using HybridCache.ApiService;
using HybridCache.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddFusionCache();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<CityRepository>();
builder.Services.AddSingleton<WeatherForecastRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapCitiesApi();
app.MapWeatherForecastApi();

app.MapDefaultEndpoints();

await app.RunAsync()
    .ConfigureAwait(false);
