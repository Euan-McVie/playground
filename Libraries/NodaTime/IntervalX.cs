using NodaTime;

namespace Libraries.NodaTime;

public static class IntervalX
{
    public static IEnumerable<Interval> FlattenConsecutive(this IReadOnlyCollection<Interval> intervals)
    {
        ArgumentNullException.ThrowIfNull(intervals);

        if (intervals.Count == 0)
        {
            yield break;
        }
        else if (intervals.Count == 1)
        {
            yield return intervals.Single();
        }

        var sortedIntervals = intervals.OrderBy(interval => interval.Start);
        var enumerator = sortedIntervals.GetEnumerator();
        _ = enumerator.MoveNext();
        var currentInterval = enumerator.Current;

        while (enumerator.MoveNext())
        {
            var nextInterval = enumerator.Current;
            if (currentInterval.End < nextInterval.Start)
            {
                yield return currentInterval;
                currentInterval = nextInterval;
            }
            else
            {
                currentInterval = new Interval(currentInterval.Start, nextInterval.End);
            }
        }

        yield return currentInterval;
    }

    public static IEnumerable<Interval> FlattenConsecutive2(this IReadOnlyCollection<Interval> intervals)
    {
        ArgumentNullException.ThrowIfNull(intervals);

        if (intervals.Count == 0)
        {
            return Enumerable.Empty<Interval>();
        }
        else if (intervals.Count == 1)
        {
            return new[] { intervals.Single() };
        }

        var sortedIntervals = intervals.OrderBy(interval => interval.Start).ToArray();
        var result = new List<Interval>();
        var currentInterval = sortedIntervals[0];

        for (int i = 1; i < sortedIntervals.Length; i++)
        {
            var nextInterval = sortedIntervals[i];
            if (currentInterval.End < nextInterval.Start)
            {
                result.Add(currentInterval);
                currentInterval = nextInterval;
            }
            else
            {
                currentInterval = new Interval(currentInterval.Start, nextInterval.End);
            }
        }

        result.Add(currentInterval);

        return result;
    }
}
