using Ganss.Xss;

namespace BlogWeb.Services;

/// <summary>
/// Server-side HTML sanitizer for post bodies authored in the admin editor.
/// Ported 1:1 from src/lib/sanitize.ts — same allow-list, so old post bodies
/// already in the DB keep rendering exactly as they did on the Next.js site.
/// </summary>
public static class PostHtmlSanitizer
{
    public static string Sanitize(string html)
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        foreach (var tag in new[]
        {
            "p", "br", "hr", "h2", "h3", "strong", "b", "em", "i", "s", "u",
            "ul", "ol", "li", "blockquote", "a", "img", "figure", "figcaption",
            "div", "code", "pre",
        })
        {
            sanitizer.AllowedTags.Add(tag);
        }

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("target");
        sanitizer.AllowedAttributes.Add("rel");
        sanitizer.AllowedAttributes.Add("src");
        sanitizer.AllowedAttributes.Add("alt");
        sanitizer.AllowedAttributes.Add("data-width");
        sanitizer.AllowedAttributes.Add("data-gallery");

        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("mailto");

        sanitizer.PostProcessNode += (s, e) =>
        {
            if (e.Node is AngleSharp.Dom.IElement el && el.TagName == "A")
            {
                el.SetAttribute("rel", "noopener noreferrer");
                el.SetAttribute("target", "_blank");
            }
        };

        return sanitizer.Sanitize(html);
    }
}
