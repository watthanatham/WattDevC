using BlogWeb.Data;
using BlogWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB connection string + Supabase keys live in appsettings.Local.json
// (gitignored, sits next to appsettings.json — see appsettings.Local.json.example).
// Loaded unconditionally so it works the same way regardless of how
// ASPNETCORE_ENVIRONMENT ends up set for a given launch method.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Default is not set — copy appsettings.Local.json.example to appsettings.Local.json and fill it in.");

builder.Services.AddDbContext<BlogDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<SupabaseAuthService>();

// Admin auth: credentials are verified against Supabase (see SupabaseAuthService),
// but the session itself is a plain ASP.NET cookie — mirrors verifySession()
// in src/lib/dal.ts, which just checks for a logged-in Supabase user.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BlogWebAdmin";
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
