using BenchmarkDotNet.Attributes;

namespace Benchmarks.Tasks;

[MemoryDiagnoser]
public class TaskForwarding
{
    [Benchmark]
    public async Task TaskForwardingWithAwait()
    {
        _ = await TaskWithAwait().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task TaskForwardingWithoutAwait()
    {
        _ = await TaskWithoutAwait().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task DoubleTaskForwardingWithAwait()
    {
        _ = await DoubleTaskWithAwait().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task DoubleTaskForwardingWithoutAwait()
    {
        _ = await DoubleTaskWithoutAwait().ConfigureAwait(false);
    }

    private async Task<int> TaskWithAwait()
    {
        return await DoWorkAsync().ConfigureAwait(false);
    }

    private async Task<int> DoubleTaskWithAwait()
    {
        return await TaskWithAwait().ConfigureAwait(false);
    }

    private Task<int> TaskWithoutAwait()
    {
        return DoWorkAsync();
    }

    private Task<int> DoubleTaskWithoutAwait()
    {
        return TaskWithoutAwait();
    }

    private Task<int> DoWorkAsync() => Task.FromResult(1);
}
