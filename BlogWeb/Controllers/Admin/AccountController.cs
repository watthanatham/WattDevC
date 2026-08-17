using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BlogWeb.Models;
using BlogWeb.Services;

namespace BlogWeb.Controllers.Admin;

/// <summary>Admin login/logout — ported from src/lib/actions/auth.ts.</summary>
public class AccountController : Controller
{
    private readonly SupabaseAuthService _auth;

    public AccountController(SupabaseAuthService auth)
    {
        _auth = auth;
    }

    [HttpGet("admin/login")]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost("admin/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        var session = await _auth.SignInAsync(vm.Email, vm.Password);
        if (session == null)
        {
            vm.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
            return View(vm);
        }

        var claims = new[] { new Claim(ClaimTypes.Email, vm.Email) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // Not HttpOnly on purpose: supabase-js in the browser reads these to call
        // auth.setSession(), so image uploads can go straight to Supabase Storage
        // as this same authenticated user (see SupabaseSession).
        var cookieOptions = new CookieOptions { Secure = Request.IsHttps, SameSite = SameSiteMode.Lax, Path = "/admin" };
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
