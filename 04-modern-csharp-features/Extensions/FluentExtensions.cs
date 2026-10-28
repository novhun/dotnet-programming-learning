namespace ModernCSharp.Extensions;

using System.Text.RegularExpressions;

public static class FluentExtensions
{
    /// <summary>
    /// Checks if a comparable value is between a min and max (inclusive).
    /// </summary>
    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// Converts a title string into a URL-friendly slug.
    /// </summary>
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        
        string lower = input.ToLowerInvariant().Trim();
        string clean = Regex.Replace(lower, @"[^a-z0-9\s-]", "");
        return Regex.Replace(clean, @"\s+", "-");
    }

    /// <summary>
    /// Safely truncates a string to a max length with an ellipsis.
    /// </summary>
    public static string Truncate(this string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength) return input;
        return input[..Math.Max(0, maxLength - 3)] + "...";
    }
}
