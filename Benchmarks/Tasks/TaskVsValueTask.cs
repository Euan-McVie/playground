using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Tasks;

[MemoryDiagnoser]
public class TaskVsValueTask
{
    [Params(1_000, 4_000, 10_000)]
    public int TaskCount { get; set; }

    private bool AlwaysTrue { get; } = true;

    [Benchmark]
    public async Task TaskSyncOperation()
    {
        for (int i = 0; i < TaskCount; i++)
        {
            await SyncTaskOp().ConfigureAwait(false);
        }
    }

    [Benchmark]
    public async Task TaskAsyncOperation()
    {
        for (int i = 0; i < TaskCount; i++)
        {
            await AsyncTaskOp().ConfigureAwait(false);
        }
    }

    [Benchmark]
    public async Task ValueTaskSyncOperation()
    {
        for (int i = 0; i < TaskCount; i++)
        {
            await SyncValueTaskOp().ConfigureAwait(false);
        }
    }

    [Benchmark]
    public async Task ValueTaskAsyncOperation()
    {
        for (int i = 0; i < TaskCount; i++)
        {
            await AsyncValueTaskOp().ConfigureAwait(false);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task<string> TaskOp() => Task.FromResult("Done");

    private async Task<string> SyncTaskOp()
    {
        if (AlwaysTrue)
        {
            return "Done";
        }

        return await TaskOp().ConfigureAwait(false);
    }

    private async Task<string> AsyncTaskOp()
    {
        if (!AlwaysTrue)
        {
            return "Done";
        }

        return await TaskOp().ConfigureAwait(false);
    }

    private async ValueTask<string> SyncValueTaskOp()
    {
        if (AlwaysTrue)
        {
            return "Done";
        }

        return await TaskOp().ConfigureAwait(false);
    }

    private async ValueTask<string> AsyncValueTaskOp()
    {
        if (!AlwaysTrue)
        {
            return "Done";
        }

        return await TaskOp().ConfigureAwait(false);
    }
}
