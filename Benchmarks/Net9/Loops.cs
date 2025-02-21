using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class Loops
{
    private readonly int[] _array = Enumerable.Range(0, 1000).ToArray();

    [Benchmark]
    public int InDirectSum()
    {
        int[] array = _array;
        int sum = 0;

        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }

        return sum;
    }

    [Benchmark]
    public int DirectSum()
    {
        int sum = 0;

        for (int i = 0; i < _array.Length; i++)
        {
            sum += _array[i];
        }

        return sum;
    }

    [Benchmark]
    public int UpwardCounting()
    {
        int count = 0;

        for (int i = 0; i < 100; i++)
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int DownwardCounting()
    {
        int count = 0;

        for (int i = 99; i >= 0; i--)
        {
            count++;
        }

        return count;
    }
}
