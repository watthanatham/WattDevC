using System.Text.RegularExpressions;

namespace BlogWeb.Services;

/// <summary>Shared blog constants + helpers, ported 1:1 from src/lib/blog.ts.</summary>
public static class BlogHelpers
{
    public static readonly (string Value, string Label)[] Categories =
    {
        ("Street", "Street"),
        ("Portrait", "Portrait"),
        ("Fashion", "Fashion"),
        ("General", "General"),
    };

    public static string CategoryLabel(string value) =>
        Categories.FirstOrDefault(c => c.Value == value).Label ?? value;

    /// <summary>
    /// Rough reading time in minutes from the post's HTML body. Strips tags so
    /// image URLs / markup don't inflate the count, then counts words at 200 wpm.
    /// </summary>
    public static int ReadingTimeMinutes(string html)
    {
        var text = Regex.Replace(html, "<[^>]*>", " ");
        text = text.Replace("&nbsp;", " ");
        text = Regex.Replace(text, "&[a-z]+;", " ", RegexOptions.IgnoreCase);
        var words = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Round(words / 200.0));
    }
}
