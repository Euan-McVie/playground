using BenchmarkDotNet.Attributes;

namespace Benchmarks.Collections;

[MemoryDiagnoser]
public class CollectionExpressions
{
    private readonly IEnumerable<int> _testData = Enumerable.Range(1, 10_000).ToArray();

    [Benchmark(Baseline = true)]
    public IReadOnlyList<int> WhereToArray()
    {
        return _testData
            .Where(x => x % 2 == 0)
            .ToArray();
    }

    [Benchmark]
    public IReadOnlyList<int> WhereToList()
    {
        return _testData
            .Where(x => x % 2 == 0)
            .ToList();
    }

    [Benchmark]
    public IReadOnlyList<int> WhereCollectionExpression()
    {
        return [.. _testData.Where(x => x % 2 == 0)];
    }
}
