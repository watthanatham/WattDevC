namespace BlogWeb.Services;

/// <summary>
/// Admin-entered links go straight into href="" / src="". Anything that isn't
/// http(s) — javascript:, data: — would execute when clicked, and the tables
/// these come from are writable outside the app, so the check has to be
/// server-side (&lt;input type="url"&gt; only guards the browser).
/// </summary>
public static class SafeUrl
{
    public static string? Clean(string? value)
    {
        var url = (value ?? "").Trim();
        if (url.Length == 0) return null;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? url
            : null;
    }
}
