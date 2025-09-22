using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[MemoryDiagnoser]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class ListExpressions
{
    public static char RightSeparator { get; set; } = ']';

    [Params("Hello World", "Hello World ", null, "", "Hello World]")]
    public static string? TestString { get; set; }

    [Benchmark(Baseline = true)]
    public bool IfStyleChecks() =>
        !string.IsNullOrEmpty(TestString)
        && TestString[^1] != ' '
        && TestString[^1] != RightSeparator;

    [Benchmark]
    public bool SwitchStyleChecks() => TestString switch
    {
        null => false,
        "" => false,
        [.., ' '] => false,
        [.., var lastChar] when lastChar == RightSeparator => false,
        _ => true,
    };
}
