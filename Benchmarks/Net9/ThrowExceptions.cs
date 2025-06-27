using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[MemoryDiagnoser(false)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class ThrowExceptions
{
    [Benchmark]
    public async Task ExceptionThrowCatch()
    {
        for (int i = 0; i < 1000; i++)
        {
            try
            {
                await Recur(10).ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
            }
        }
    }

    private async Task Recur(int depth)
    {
        if (depth <= 0)
        {
            await Task.Yield();
            throw new InvalidOperationException("Test");
        }

        await Recur(depth - 1).ConfigureAwait(false);
    }
}
