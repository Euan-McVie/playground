using BenchmarkDotNet.Attributes;

namespace Benchmarks.Linq;

[MemoryDiagnoser]
public class CollectionExpressions
{
    private readonly IEnumerable<int> _testData = Enumerable.Range(1, 10_000).ToArray();

    [Benchmark(Baseline = true)]
    public IReadOnlyList<int> WhereToArray() => _testData
        .Where(x => x % 2 == 0)
        .ToArray();

    [Benchmark]
    public IReadOnlyList<int> WhereToList() => _testData
        .Where(x => x % 2 == 0)
        .ToList();

    [Benchmark]
    public IReadOnlyList<int> WhereCollectionExpression() => [.. _testData.Where(x => x % 2 == 0)];
}
