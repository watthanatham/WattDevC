using System.Security.Cryptography;
using System.Threading.RateLimiting;
using BlogWeb.Data;
using BlogWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB connection string + Supabase keys live in appsettings.Local.json
// (gitignored, sits next to appsettings.json — see appsettings.Local.json.example).
// Loaded unconditionally so it works the same way regardless of how
// ASPNETCORE_ENVIRONMENT ends up set for a given launch method.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Default is not set — copy appsettings.Local.json.example to appsettings.Local.json and fill it in.");

builder.Services.AddDbContext<BlogDbContext>(options => options.UseNpgsql(connectionString));

// Fail fast rather than silently locking every admin out (or, worse, letting
// anyone with a Supabase account in — see AccountController).
_ = builder.Configuration["Admin:AllowedEmails"]
    ?? throw new InvalidOperationException(
        "Admin:AllowedEmails is not set — list the admin email(s), comma-separated.");

builder.Services.AddHttpClient<SupabaseAuthService>();

// Render/Railway terminate TLS at their proxy and forward plain HTTP inside the
// container. Without this Request.IsHttps is false in production and every
// auth cookie ships without its Secure flag.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // The container is only reachable through the platform's proxy, whose
    // address is not known ahead of time.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Brute-force guard on the login form — Supabase itself has no per-IP limit here.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(5) }));
});

// Admin auth: credentials are verified against Supabase (see SupabaseAuthService),
// but the session itself is a plain ASP.NET cookie — mirrors verifySession()
// in src/lib/dal.ts, which just checks for a logged-in Supabase user.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BlogWebAdmin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        // Always, not the SameAsRequest default — see the ForwardedHeaders note above.
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseForwardedHeaders();

// Everything but the nonce is fixed per deployment, so build the policy once.
// Inline <script> blocks carry the nonce; inline style="" attributes are all
// over the views, so styles stay 'unsafe-inline' (CSS is not a script vector
// here). Images allow any https origin so existing post content keeps loading.
var supabaseOrigin = app.Configuration["Supabase:Url"]?.TrimEnd('/') ?? "";
var cspTemplate = string.Join("; ",
    "default-src 'self'",
    "script-src 'self' 'nonce-{0}' https://cdn.jsdelivr.net",
    "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com",
    "font-src 'self' https://fonts.gstatic.com",
    "img-src 'self' data: https:",
    $"connect-src 'self' {supabaseOrigin}".TrimEnd(),
    "form-action 'self'",
    "frame-ancestors 'none'",
    "base-uri 'self'",
    "object-src 'none'");

app.Use(async (context, next) =>
{
    // Hex, not base64: Razor html-encodes "+" to "&#x2B;" in the nonce="" attribute,
    // which silently breaks the match with this header for ~30% of requests.
    var nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    context.Items["csp-nonce"] = nonce;

    var headers = context.Response.Headers;
    headers["Content-Security-Policy"] = string.Format(cspTemplate, nonce);
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
