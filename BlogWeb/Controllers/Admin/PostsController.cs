using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;
using BlogWeb.Services;

namespace BlogWeb.Controllers.Admin;

/// <summary>Posts CRUD — ported from src/lib/actions/posts.ts.</summary>
[Authorize]
public class PostsController : Controller
{
    private readonly BlogDbContext _db;

    public PostsController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpGet("admin/posts")]
    public async Task<IActionResult> Index()
    {
        var posts = await _db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return View(posts);
    }

    [HttpGet("admin/posts/new")]
    public IActionResult New() => View("Form", new PostFormViewModel());

    [HttpPost("admin/posts/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostFormViewModel vm)
    {
        // A form field submitted with an empty value binds to null, not "" —
        // every plain string field here needs the same guard.
        vm.Title = (vm.Title ?? "").Trim();
        var excerpt = (vm.Excerpt ?? "").Trim();
        var coverImage = (vm.CoverImage ?? "").Trim();
        var body = vm.Body ?? "";

        if (string.IsNullOrEmpty(vm.Title) || IsEmptyHtml(body))
        {
            vm.Error = "กรุณากรอกชื่อเรื่องและเนื้อหา";
            return View("Form", vm);
        }

        var slug = await UniqueSlugAsync(vm.Title, null);

        _db.Posts.Add(new Post
        {
            Title = vm.Title,
            Slug = slug,
            Body = body,
            Category = vm.Category ?? "Street",
            Excerpt = excerpt.Length > 0 ? excerpt : null,
            Tags = ParseTags(vm.Tags),
            Published = vm.Published,
            CoverImage = coverImage.Length > 0 ? coverImage : null,
        });
        await _db.SaveChangesAsync();

        return Redirect("/admin/posts");
    }

    [HttpGet("admin/posts/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return NotFound();

        var vm = new PostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt ?? "",
            Category = post.Category,
            Tags = string.Join(", ", post.Tags ?? new List<string>()),
            Published = post.Published,
            CoverImage = post.CoverImage ?? "",
            Body = post.Body,
        };
        return View("Form", vm);
    }

    [HttpPost("admin/posts/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PostFormViewModel vm)
    {
        // A form field submitted with an empty value binds to null, not "" —
        // every plain string field here needs the same guard.
        vm.Title = (vm.Title ?? "").Trim();
        var excerpt = (vm.Excerpt ?? "").Trim();
        var body = vm.Body ?? "";

        if (string.IsNullOrEmpty(vm.Title) || IsEmptyHtml(body))
        {
            vm.Error = "กรุณากรอกชื่อเรื่องและเนื้อหา";
            return View("Form", vm);
        }

        var existing = await _db.Posts.FindAsync(id);
        if (existing == null)
        {
            vm.Error = "ไม่พบบทความนี้";
            return View("Form", vm);
        }

        // Browser-uploaded cover URL; falls back to the existing one if unchanged.
        var coverImage = (vm.CoverImage ?? "").Trim();
        existing.CoverImage = coverImage.Length > 0 ? coverImage : existing.CoverImage;

        existing.Slug = vm.Title == existing.Title ? existing.Slug : await UniqueSlugAsync(vm.Title, id);
        existing.Title = vm.Title;
        existing.Body = body;
        existing.Category = vm.Category ?? "Street";
        existing.Excerpt = excerpt.Length > 0 ? excerpt : null;
        existing.Tags = ParseTags(vm.Tags);
        existing.Published = vm.Published;

        await _db.SaveChangesAsync();
        return Redirect("/admin/posts");
    }

    [HttpPost("admin/posts/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post != null)
        {
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/posts");
    }

    /// <summary>True when the editor HTML has no real content (empty &lt;p&gt;&lt;/p&gt;, no images).</summary>
    private static bool IsEmptyHtml(string html)
    {
        var hasImage = Regex.IsMatch(html, "<img\\b", RegexOptions.IgnoreCase);
        var text = Regex.Replace(html, "<[^>]*>", "").Replace("&nbsp;", " ").Trim();
        return !hasImage && text.Length == 0;
    }

    /// <summary>Comma-separated tag input → clean, de-duplicated string list.</summary>
    private static List<string> ParseTags(string? raw) =>
        (raw ?? "").Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).Distinct().ToList();

    private async Task<string> UniqueSlugAsync(string title, int? ignoreId)
    {
        var slugBase = Slugify.ToSlug(title);
        if (slugBase.Length == 0) slugBase = $"post-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var slug = slugBase;
        var n = 1;
        while (await _db.Posts.AnyAsync(p => p.Slug == slug && (ignoreId == null || p.Id != ignoreId)))
        {
            n += 1;
            slug = $"{slugBase}-{n}";
        }
        return slug;
    }
}
