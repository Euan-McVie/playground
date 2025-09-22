using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Libraries.NodaTime;
using NodaTime;

namespace Benchmarks.NodaTime;

[SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Security not required")]
[MemoryDiagnoser]
public class FlattenConsecutiveIntervals
{
    private static readonly Instant s_now = SystemClock.Instance.GetCurrentInstant();
    private readonly Consumer _consumer = new();
    private Interval[] _testData = null!;

    [Params(10, 100, 1_000)]
    public static int IntervalCount { get; set; }

    [ParamsSource(nameof(GenSeed))]
    public static int Seed { get; set; }

    public static IEnumerable<int> GenSeed => [Random.Shared.Next()];

    [GlobalSetup]
    public void SetupData()
    {
        var random = new Random(Seed);

        Duration GetDayAdjustment(int offset) => Duration.FromDays(offset + random.Next(3));

        _testData = Enumerable
            .Range(1, IntervalCount)
            .Select(offset =>
            {
                var start = s_now + GetDayAdjustment(offset);
                return new Interval(start, start + GetDayAdjustment(offset + 1));
            })
            .ToArray();
    }

    [Benchmark]
    public void FlattenConsecutiveIntervals1() => _testData.FlattenConsecutive().Consume(_consumer);

    [Benchmark]
    public void FlattenConsecutiveIntervals2() => _testData.FlattenConsecutive2().Consume(_consumer);
}
