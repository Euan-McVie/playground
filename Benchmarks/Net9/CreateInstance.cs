using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Net9;

[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD", "NuGetReferences")]

public class CreateInstance
{
    private readonly IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

    [Benchmark]
    public TestClass Create() => ActivatorUtilities.CreateInstance<TestClass>(_serviceProvider, 1, 2, 3);
}
