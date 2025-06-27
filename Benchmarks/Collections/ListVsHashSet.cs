using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Collections;

[SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Security not required")]
public class ListVsHashSet
{
    private readonly int _notPresent = -1;
    private HashSet<int> _hashSet = default!;
    private List<int> _list = default!;
    private int _first;
    private int _last;

    [Params(5, 15)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void SetupData()
    {
        _list = Enumerable.Range(1, ItemCount).ToList();
        _first = _list.First();
        _last = _list.Last();

        _hashSet = _list.ToHashSet();
    }

    // [Benchmark]
    public bool ListContainsFirst()
    {
        return _list.Contains(_first);
    }

    // [Benchmark]
    public bool HashSetContainsFirst()
    {
        return _hashSet.Contains(_first);
    }

    [Benchmark(Baseline = true)]
    public bool ListContainsLast()
    {
        return _list.Contains(_last);
    }

    [Benchmark]
    public bool HashSetContainsLast()
    {
        return _hashSet.Contains(_last);
    }

    [Benchmark]
    public bool ListDoesNotContain()
    {
        return _list.Contains(_notPresent);
    }

    [Benchmark]
    public bool HashSetDoesNotContain()
    {
        return _hashSet.Contains(_notPresent);
    }
}
