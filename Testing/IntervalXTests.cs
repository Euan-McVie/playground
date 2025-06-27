using System.Diagnostics.CodeAnalysis;
using Libraries.NodaTime;
using NodaTime;
using Shouldly;

namespace Testing;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test class")]
public class IntervalXTests
{
    private readonly Interval[] _givenIntervals = [
        new(
            Instant.FromUtc(2024, 01, 01, 00, 00),
            Instant.FromUtc(2024, 01, 02, 00, 00)),
        new(
            Instant.FromUtc(2024, 01, 02, 00, 00),
            Instant.FromUtc(2024, 01, 03, 00, 00)),
        new(
            Instant.FromUtc(2024, 01, 04, 00, 00),
            Instant.FromUtc(2024, 01, 05, 00, 00)),
        ];

    private readonly Interval[] _expectedIntervals = [
        new(
            Instant.FromUtc(2024, 01, 01, 00, 00),
            Instant.FromUtc(2024, 01, 03, 00, 00)),
        new(
            Instant.FromUtc(2024, 01, 04, 00, 00),
            Instant.FromUtc(2024, 01, 05, 00, 00)),
        ];

    [Fact]
    public void Given_consecutive_intervals_When_flattening_Then_combine_consecutive_intervals()
    {
        // Act
        var result = _givenIntervals.FlattenConsecutive();

        // Assert
        result.ShouldBe(_expectedIntervals);
    }

    [Fact]
    public void Given_consecutive_intervals_When_flattening_2_Then_combine_consecutive_intervals()
    {
        // Act
        var result = _givenIntervals.FlattenConsecutive2();

        // Assert
        result.ShouldBe(_expectedIntervals);
    }
}
