namespace Testing;

public class VerifyImmutableDictionaryTests
{
    [Fact(Skip ="Broken in Verify")]
    public Task DictionaryOrderNonComparable()
    {
        var dictionary = new Dictionary<NonComparableKey, string>
        {
            [new("Foo2")] = "Bar",
            [new("Foo1")] = "Bar",
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task DictionaryOrderComparable()
    {
        var dictionary = new Dictionary<ComparableKey, string>
        {
            [new("Foo2")] = "Bar",
            [new("Foo1")] = "Bar",
        };

        return Verify(dictionary);
    }

    private sealed record NonComparableKey(string Member)
    {
        public override string ToString() => Member;

#pragma warning disable CA1307 // Specify StringComparison for clarity
        public override int GetHashCode() => Member.GetHashCode();
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    private sealed record ComparableKey(string Member) : IComparable, IComparable<ComparableKey>
    {
        public override string ToString() => Member;

#pragma warning disable CA1307 // Specify StringComparison for clarity
        public override int GetHashCode() => Member.GetHashCode();
#pragma warning restore CA1307 // Specify StringComparison for clarity

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is not ComparableKey other)
            {
                throw new ArgumentException($"Must be {nameof(ComparableKey)}");
            }

            return CompareTo(other);
        }

#pragma warning disable CA1310 // Specify StringComparison for correctness
        public int CompareTo(ComparableKey? other) => Member.CompareTo(other?.Member);
#pragma warning restore CA1310 // Specify StringComparison for correctness
    }
}
