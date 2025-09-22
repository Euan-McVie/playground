#pragma warning disable S125 // Sections of code should not be commented out

using System.Runtime.CompilerServices;

namespace ScratchPad;

internal static class ParallelTaskExceptions
{
    internal static async Task Run()
    {
        Console.WriteLine("\nAsyncWork: Parallel");
        await Parallel(() => AsyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nAsyncWork: ParallelInternalAwait");
        await ParallelInternalAwait(() => AsyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nAsyncWork: ParallelExplicitToArray");
        await ParallelExplicitToArray(() => AsyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nAsyncWork: ParallelInternalAwaitAndExplicitToArray");
        await ParallelInternalAwaitAndExplicitToArray(() => AsyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nSyncWork: Parallel");
        await Parallel(() => SyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nSyncWork: ParallelInternalAwait");
        await ParallelInternalAwait(() => SyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nSyncWork: ParallelExplicitToArray");
        await ParallelExplicitToArray(() => SyncWork()).ConfigureAwait(false);

        Console.WriteLine("\nSyncWork: ParallelInternalAwaitAndExplicitToArray");
        await ParallelInternalAwaitAndExplicitToArray(() => SyncWork()).ConfigureAwait(false);
    }

    private static async Task Parallel(Func<Task> work)
    {
        IEnumerable<Task> items;
        try
        {
            items = Enumerable.Range(0, 10)
                .Select(_ => work());
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Select exception");
            return;
        }

        try
        {
            var task = Task.WhenAll(items);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"WhenAll exception. Count: {task.Exception?.InnerExceptions.Count}");
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"WhenAll exception");
        }
    }

    private static async Task ParallelInternalAwait(Func<Task> work)
    {
        IEnumerable<Task> items;
        try
        {
            items = Enumerable.Range(0, 10)
                .Select(async _ => await work().ConfigureAwait(false));
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Select exception");
            return;
        }

        try
        {
            var task = Task.WhenAll(items);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"WhenAll exception. Count: {task.Exception?.InnerExceptions.Count}");
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"WhenAll exception");
        }
    }

    private static async Task ParallelExplicitToArray(Func<Task> work)
    {
        IEnumerable<Task> items;
        try
        {
            items = Enumerable.Range(0, 10)
                .Select(_ => work())
                .ToArray();
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Select exception");
            return;
        }

        try
        {
            var task = Task.WhenAll(items);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Awaiting Task exception. Count: {task.Exception?.InnerExceptions.Count}");
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"WhenAll exception");
        }
    }

    private static async Task ParallelInternalAwaitAndExplicitToArray(Func<Task> work)
    {
        IEnumerable<Task> items;
        try
        {
            items = Enumerable.Range(0, 10)
                .Select(async _ => await work().ConfigureAwait(false))
                .ToArray();
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Select exception");
            return;
        }

        try
        {
            var task = Task.WhenAll(items);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"WhenAll exception. Count: {task.Exception?.InnerExceptions.Count}");
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"WhenAll exception");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task AsyncWork(bool throwException = true)
    {
        if (throwException)
        {
            throw new InvalidOperationException("Exception from AsyncMethod");
        }

        await SyncWork(false).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Task SyncWork(bool throwException = true)
        => throwException
        ? throw new InvalidOperationException("Exception from AsyncMethod")
        : AsyncWork(false);
}
#pragma warning restore S125 // Sections of code should not be commented out
