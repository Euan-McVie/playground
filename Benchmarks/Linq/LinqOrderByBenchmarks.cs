using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Benchmarks.Linq;

[MemoryDiagnoser]
public class LinqOrderByBenchmarks
{
    private readonly (int A, int B)[] _testData = Enumerable
        .Range(1, 1_000)
        .Select(x => (x, x * x))
        .ToArray();

    private readonly Consumer _consumer = new();

    [Benchmark(Baseline = true)]
    public void OrderByValueTuple() => _testData
        .OrderBy(x => (x.A, x.B))
        .Consume(_consumer);

    [Benchmark]
    public void OrderByThenBy() => _testData
        .OrderBy(x => x.A)
        .ThenBy(x => x.B)
        .Consume(_consumer);
}
