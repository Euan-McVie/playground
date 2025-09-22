using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class StackAlloc
{
    [Benchmark]
    public int ReturnValue() => new MyObj(42).Value;

    private sealed class MyObj(int value)
    {
        public int Value { get; } = value;
    }
}
