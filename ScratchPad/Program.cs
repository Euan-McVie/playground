using System.Diagnostics.CodeAnalysis;

Generator? generatorA = new("A");
Generator? generator2ndA = new("A");
Generator? generatorZ = new("Z");
Generator? generatorNull = null;
Generator? generator2ndNull = null;

Console.WriteLine($"{generatorA} < {generatorZ} = {generatorA < generatorZ}");
Console.WriteLine($"{generatorZ} < {generatorA} = {generatorZ < generatorA}");
Console.WriteLine($"{generatorA} < {generator2ndA} = {generatorA < generator2ndA}");
Console.WriteLine($"{generatorA} < {generatorNull} = {generatorA < generatorNull}");
Console.WriteLine($"{generatorNull} < {generatorA} = {generatorNull < generatorA}");
Console.WriteLine($"{generatorNull} < {generator2ndNull} = {generatorNull < generator2ndNull}");
Console.WriteLine("-----------------------------------");
Console.WriteLine($"{generatorA} > {generatorZ} = {generatorA > generatorZ}");
Console.WriteLine($"{generatorZ} > {generatorA} = {generatorZ > generatorA}");
Console.WriteLine($"{generatorA} > {generator2ndA} = {generatorA > generator2ndA}");
Console.WriteLine($"{generatorA} > {generatorNull} = {generatorA > generatorNull}");
Console.WriteLine($"{generatorNull} > {generatorA} = {generatorNull > generatorA}");
Console.WriteLine($"{generatorNull} > {generator2ndNull} = {generatorNull > generator2ndNull}");
Console.WriteLine("-----------------------------------");
Console.WriteLine($"{generatorA} <= {generatorZ} = {generatorA <= generatorZ}");
Console.WriteLine($"{generatorZ} <= {generatorA} = {generatorZ <= generatorA}");
Console.WriteLine($"{generatorA} <= {generator2ndA} = {generatorA <= generator2ndA}");
Console.WriteLine($"{generatorA} <= {generatorNull} = {generatorA <= generatorNull}");
Console.WriteLine($"{generatorNull} <= {generatorA} = {generatorNull <= generatorA}");
Console.WriteLine($"{generatorNull} <= {generator2ndNull} = {generatorNull <= generator2ndNull}");
Console.WriteLine("-----------------------------------");
Console.WriteLine($"{generatorA} >= {generatorZ} = {generatorA >= generatorZ}");
Console.WriteLine($"{generatorZ} >= {generatorA} = {generatorZ >= generatorA}");
Console.WriteLine($"{generatorA} >= {generator2ndA} = {generatorA >= generator2ndA}");
Console.WriteLine($"{generatorA} >= {generatorNull} = {generatorA >= generatorNull}");
Console.WriteLine($"{generatorNull} >= {generatorA} = {generatorNull >= generatorA}");
Console.WriteLine($"{generatorNull} >= {generator2ndNull} = {generatorNull >= generator2ndNull}");

[SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "Testing")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Testing")]
public record Generator(string Identifier)
    : IComparable, IComparable<Generator>
{
    public static bool operator <(Generator? left, Generator? right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Generator? left, Generator? right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Generator? left, Generator? right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Generator? left, Generator? right)
        => left is null ? right is null : left.CompareTo(right) >= 0;

    public virtual bool Equals(Generator? other)
    {
        if (other is null)
        {
            return false;
        }

        return Identifier.Equals(other.Identifier, StringComparison.Ordinal);
    }

    public override int GetHashCode() => Identifier.GetHashCode(StringComparison.Ordinal);

    public int CompareTo(Generator? other)
        => string.CompareOrdinal(Identifier.ToString(), other?.Identifier.ToString());

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not Generator other)
        {
            throw new ArgumentException($"Must be a {nameof(Generator)}", nameof(obj));
        }

        return CompareTo(other);
    }
}
