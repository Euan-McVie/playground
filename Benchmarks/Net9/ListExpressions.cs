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
    public bool IfStyleChecks()
    {
        return !string.IsNullOrEmpty(TestString)
            && TestString[^1] != ' '
            && TestString[^1] != RightSeparator;
    }

    [Benchmark]
    public bool SwitchStyleChecks()
    {
        return TestString switch
        {
            null => false,
            "" => false,
#if NET8_0_OR_GREATER
            [.., ' '] => false,
            [.., var lastChar] when lastChar == RightSeparator => false,
#endif
            _ => true,
        };
    }
}
