using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Tasks;

[MemoryDiagnoser]
public class TaskWhenAll
{
    private IEnumerable<Task> _enumerable = default!;

    [Params(10, 1_000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void SetupData()
        => _enumerable = Enumerable.Range(1, ItemCount)
            .Select(async item => await AsyncMethod().ConfigureAwait(false));

    [Benchmark]
    public async Task WhenAllEnumerable()
        => await Task.WhenAll(_enumerable)
            .ConfigureAwait(false);

    [Benchmark]
    public async Task WhenAllList()
    {
        var list = _enumerable.ToList();

        await Task.WhenAll(list)
            .ConfigureAwait(false);
    }

    [Benchmark]
    public async Task WhenAllArray()
    {
        var array = _enumerable.ToArray();

        await Task.WhenAll(array)
            .ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task AsyncMethod()
        => await Task.Yield();
}
