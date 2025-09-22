using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Regexes;

/// <summary>
/// Provides functionality to benchmark extracting specific suffixes from a given path string based on predefined patterns and
/// conditions.
/// </summary>
[MemoryDiagnoser]
public partial class ExtractSuffix
{
    private static readonly string s_localViewPattern = $"/{Topic.LocalView}/";

    private enum Topic
    {
        LocalView,
    }

    [Params("/user/username/version/streaming/contracts", "/user/username/version/streaming/localview/12")]
    public required string Path { get; set; }

    [Benchmark(Baseline = true)]
    public string? ExtractWithIndexOf()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return null;
        }

        var match = Regexes.ExtractSuffixPattern().Match(Path);
        if (!match.Success)
        {
            return null;
        }

        string? fullSuffix = match.Groups[1].Value;
        if (string.IsNullOrEmpty(fullSuffix))
        {
            return null;
        }

        // Special case: if path contains "localview", return "/localview/<deliveryAreaId>"
        int localViewIndex = fullSuffix.IndexOf(
            s_localViewPattern,
            StringComparison.OrdinalIgnoreCase);
        if (localViewIndex >= 0)
        {
            return fullSuffix[localViewIndex..];
        }

        // Default case: return the last segment
        int lastSlashIndex = fullSuffix.LastIndexOf('/');
        return lastSlashIndex >= 0 ? fullSuffix[lastSlashIndex..] : fullSuffix;
    }

    [Benchmark]
    public string? ExtractWithRegex()
    {
        var match = Regexes.ExtractExactSuffixPattern().Match(Path);

        return !match.Success ? null : match.Value;
    }

    private static partial class Regexes
    {
        [GeneratedRegex("^/user/[^/]+/[^/]+(.*)$", RegexOptions.IgnoreCase)]
        public static partial Regex ExtractSuffixPattern();

        [GeneratedRegex(@"\/(localview\/\d+|\w+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture)]
        public static partial Regex ExtractExactSuffixPattern();
    }
}
