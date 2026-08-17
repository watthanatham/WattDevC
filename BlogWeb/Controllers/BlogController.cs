using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;
using BlogWeb.Services;

namespace BlogWeb.Controllers;

public class BlogController : Controller
{
    private const int PageSize = 9;
    private readonly BlogDbContext _db;

    public BlogController(BlogDbContext db)
    {
        _db = db;
    }

    // GET /blog?page=&q=&cat=
    public async Task<IActionResult> Index(int? page, string? q, string? cat)
    {
        var currentPage = Math.Max(1, page ?? 1);
        var search = (q ?? "").Trim();
        var category = BlogHelpers.Categories.Any(c => c.Value == cat) ? cat! : "";

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Id == 1);

        var query = _db.Posts.Where(p => p.Published);
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => EF.Functions.ILike(p.Title, $"%{search}%"));
        }
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        var total = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var vm = new BlogListViewModel
        {
            Posts = posts,
            Page = currentPage,
            TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize)),
            Search = search,
            Category = category,
            Author = !string.IsNullOrEmpty(profile?.PenName) ? profile.PenName! : (profile?.Name ?? "Journal"),
            PenBio = profile?.PenBio,
        };

        return View(vm);
    }

    // GET /blog/{slug}
    [Route("blog/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug && p.Published);
        if (post == null) return NotFound();

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Id == 1);
        var related = await _db.Posts
            .Where(p => p.Published && p.Id != post.Id && p.Category == post.Category)
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        var vm = new BlogDetailViewModel
        {
            Post = post,
            Author = !string.IsNullOrEmpty(profile?.PenName) ? profile.PenName! : (profile?.Name ?? "Anonymous"),
            PenAvatarUrl = profile?.PenAvatarUrl,
            ReadingMinutes = BlogHelpers.ReadingTimeMinutes(post.Body),
            Related = related,
        };

        return View(vm);
    }
}
