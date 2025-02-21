// dotnet run -c Release -f net8.0 --filter "*" --runtimes net8.0 net9.0

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD", "a", "b")]
public class PGO
{
    private readonly A _obj = new C();

    [Benchmark]
    [Arguments("abcd", "abcg")]
    public bool Equals(string a, string b) => a == b;

    [Benchmark]
    public bool IsInstanceOf() => _obj is B;

    internal class A
    {
    }

    internal class B : A
    {
    }

    internal sealed class C : B
    {
    }
}
