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
        _first = _list[0];
        _last = _list[_list.Count - 1];

        _hashSet = _list.ToHashSet();
    }

    // [Benchmark]
    public bool ListContainsFirst() => _list.Contains(_first);

    // [Benchmark]
    public bool HashSetContainsFirst() => _hashSet.Contains(_first);

    [Benchmark(Baseline = true)]
    public bool ListContainsLast() => _list.Contains(_last);

    [Benchmark]
    public bool HashSetContainsLast() => _hashSet.Contains(_last);

    [Benchmark]
    public bool ListDoesNotContain() => _list.Contains(_notPresent);

    [Benchmark]
    public bool HashSetDoesNotContain() => _hashSet.Contains(_notPresent);
}
