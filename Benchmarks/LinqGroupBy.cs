using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Benchmarks;

[MemoryDiagnoser]
public class LinqGroupBy
{
    private readonly (int A, int B)[] _testData = Enumerable
        .Range(1, 1_000)
        .Select(x => (x, x * x))
        .ToArray();

    private readonly Consumer _consumer = new();

    [Benchmark]
    public void GroupByValueTuple()
    {
        _testData
            .GroupBy(x => (x.A, x.B))
            .Consume(_consumer);
    }

    [Benchmark]
    public void GroupByAnonymous()
    {
        _testData
            .GroupBy(x => new { x.A, x.B })
            .Consume(_consumer);
    }
}
