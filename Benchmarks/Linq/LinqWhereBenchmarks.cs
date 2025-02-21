using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Benchmarks.Linq;

[MemoryDiagnoser]
public class LinqWhereBenchmarks
{
    private readonly int[] _testData = Enumerable.Range(1, 1_000).ToArray();

    private readonly Consumer _consumer = new();

    [Benchmark]
    public void InlineMultipleWhere()
    {
        _testData
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x % 5 == 0)
            .Consume(_consumer);
    }

    [Benchmark]
    public void InlineCombinedWhere()
    {
        _testData
            .Where(x => x % 2 == 0 && x % 3 == 0 && x % 5 == 0)
            .Consume(_consumer);
    }

    [Benchmark]
    public void FuncMultipleWhere()
    {
        _testData
            .Where(IsDivisibleBy2)
            .Where(IsDivisibleBy3)
            .Where(IsDivisibleBy5)
            .Consume(_consumer);
    }

    [Benchmark]
    public void FuncCombinedWhere()
    {
        _testData
            .Where(x => IsDivisibleBy2(x) && IsDivisibleBy3(x) && IsDivisibleBy5(x))
            .Consume(_consumer);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool IsDivisibleBy2(int x) => x % 2 == 0;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool IsDivisibleBy3(int x) => x % 3 == 0;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool IsDivisibleBy5(int x) => x % 5 == 0;
}
