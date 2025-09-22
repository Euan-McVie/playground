using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class Locks
{
    [SuppressMessage("Style", "IDE0330:Use 'System.Threading.Lock'", Justification = "Comparison for benchmarking.")]
    private readonly object _monitor = new();

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#endif

    [Benchmark]
    public int WithMonitor()
    {
        int value = 0;

        lock (_monitor)
        {
            value++;
        }

        return value;
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public int WithLock()
    {
        int value = 0;

        lock (_lock)
        {
            value++;
        }

        return value;
    }
#endif
}
