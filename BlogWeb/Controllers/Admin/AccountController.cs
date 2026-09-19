using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using BlogWeb.Models;
using BlogWeb.Services;

namespace BlogWeb.Controllers.Admin;

/// <summary>Admin login/logout — ported from src/lib/actions/auth.ts.</summary>
public class AccountController : Controller
{
    private readonly SupabaseAuthService _auth;
    private readonly string[] _allowedEmails;

    public AccountController(SupabaseAuthService auth, IConfiguration config)
    {
        _auth = auth;
        _allowedEmails = (config["Admin:AllowedEmails"] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [HttpGet("admin/login")]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost("admin/login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        var email = (vm.Email ?? "").Trim();

        // Supabase authenticates every user in the project, not just the owner,
        // and the project allows public sign-ups — so a valid Supabase login is
        // NOT proof of being the admin. Checked before the call so the response
        // is identical whether or not the account exists.
        if (!_allowedEmails.Contains(email, StringComparer.OrdinalIgnoreCase))
        {
            vm.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
            return View(vm);
        }

        var session = await _auth.SignInAsync(email, vm.Password);
        if (session == null)
        {
            vm.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
            return View(vm);
        }

        var claims = new[] { new Claim(ClaimTypes.Email, email) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // Not HttpOnly on purpose: supabase-js in the browser reads these to call
        // auth.setSession(), so image uploads can go straight to Supabase Storage
        // as this same authenticated user (see SupabaseSession). Secure is hard-set
        // rather than tied to Request.IsHttps, which is false behind a TLS-terminating
        // proxy; browsers still accept Secure cookies on http://localhost.
        var cookieOptions = new CookieOptions { Secure = true, SameSite = SameSiteMode.Lax, Path = "/admin" };
        Response.Cookies.Append("sb-access-token", session.AccessToken, cookieOptions);
        Response.Cookies.Append("sb-refresh-token", session.RefreshToken, cookieOptions);

        return Redirect("/admin");
    }

    [HttpPost("admin/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("sb-access-token", new CookieOptions { Path = "/admin" });
        Response.Cookies.Delete("sb-refresh-token", new CookieOptions { Path = "/admin" });
        return Redirect("/admin/login");
    }
}
