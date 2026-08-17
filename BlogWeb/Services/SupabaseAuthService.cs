using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlogWeb.Services;

/// <summary>
/// Validates admin credentials against Supabase Auth — the same auth.users
/// table the Next.js app's login() action (src/lib/actions/auth.ts) uses.
/// Ported as a REST call since Supabase has no first-class .NET SDK; ASP.NET
/// then keeps its own cookie session rather than juggling the Supabase JWT.
/// </summary>
public class SupabaseAuthService
{
    private readonly HttpClient _http;
    private readonly string _anonKey;

    public SupabaseAuthService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri(config["Supabase:Url"]!.TrimEnd('/') + "/");
        _anonKey = config["Supabase:AnonKey"]!;
    }

    public async Task<SupabaseSession?> SignInAsync(string email, string password)
    {
        var payload = JsonSerializer.Serialize(new { email, password });
        using var request = new HttpRequestMessage(HttpMethod.Post, "auth/v1/token?grant_type=password")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("apikey", _anonKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);
        return new SupabaseSession(
            json.GetProperty("access_token").GetString()!,
            json.GetProperty("refresh_token").GetString()!);
    }
}

/// <summary>
/// The browser needs its own genuine Supabase session (not just our ASP.NET
/// cookie) because image uploads go straight from the browser to Supabase
/// Storage — the bucket's "authenticated insert" RLS policy checks for a real
/// Supabase JWT, which only supabase-js running in the browser can present.
/// </summary>
public record SupabaseSession(string AccessToken, string RefreshToken);
