using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class Locks
{
    private readonly object _monitor = new();

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#endif

    private int _value;

    [Benchmark]
    public void WithMonitor()
    {
        lock (_monitor)
        {
            _value++;
        }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public void WithLock()
    {
        lock (_lock)
        {
            _value++;
        }
    }
#endif
}
