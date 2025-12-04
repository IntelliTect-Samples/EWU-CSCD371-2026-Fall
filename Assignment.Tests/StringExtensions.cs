using System;
using System.Text.RegularExpressions;

namespace IntelliTect.TestTools;

public static class StringExtensions
{
    /// <summary>
    /// Returns true if the string matches the given pattern (case-insensitive).
    /// </summary>
    public static bool IsMatchRegex(this string s, string pattern)
    {
        ArgumentNullException.ThrowIfNull(s);
        ArgumentNullException.ThrowIfNull(pattern);

        return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(s);
    }

    /// <summary>
    /// Implements VB's Like operator logic.
    /// </summary>
    public static bool IsLike(this string text, string pattern)
    {
        if (text == null)
            return false;
        ArgumentNullException.ThrowIfNull(pattern);

        return new WildcardPattern(pattern).IsMatch(text);
    }

    /// <summary>
    /// Implements VB's Like operator logic, with explicit escape character.
    /// </summary>
    public static bool IsLike(this string text, string pattern, char escapeCharacter)
    {
        if (text == null)
            return false;
        ArgumentNullException.ThrowIfNull(pattern);

        return new WildcardPattern(pattern, escapeCharacter).IsMatch(text);
    }
}
