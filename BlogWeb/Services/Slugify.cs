using System.Text.RegularExpressions;

namespace BlogWeb.Services;

/// <summary>Ported 1:1 from src/lib/slugify.ts.</summary>
public static class Slugify
{
    public static string ToSlug(string text)
    {
        var s = text.ToLowerInvariant().Trim();
        s = Regex.Replace(s, @"[^\w\s-]", "");
        s = Regex.Replace(s, @"[\s_-]+", "-");
        s = Regex.Replace(s, @"^-+|-+$", "");
        return s;
    }
}
