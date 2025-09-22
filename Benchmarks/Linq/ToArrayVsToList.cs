using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Linq;

[MemoryDiagnoser]
public class ToArrayVsToList
{
    private IEnumerable<int> _enumerable = default!;

    [Params(10, 1_000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void SetupData() => _enumerable = Enumerable.Range(1, ItemCount)
            .Select(item => item + 1);

    [Benchmark]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Don't include conversion in benchmark.")]
    public List<int> ManualToList()
    {
        var list = new List<int>();
        foreach (int item in _enumerable)
        {
            list.Add(item);
        }

        return list;
    }

    [Benchmark]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Don't include conversion in benchmark.")]
    public List<int> ToList() => _enumerable.ToList();

    [Benchmark]
    public int[] ToArray() => _enumerable.ToArray();

    [Benchmark]
    public IReadOnlyCollection<int> CollectionExpression() => [.. _enumerable];
}
