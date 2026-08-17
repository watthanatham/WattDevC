using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;

namespace BlogWeb.Controllers.Admin;

[Authorize]
public class DashboardController : Controller
{
    private readonly BlogDbContext _db;

    public DashboardController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpGet("admin")]
    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            PostCount = await _db.Posts.CountAsync(),
            PublishedCount = await _db.Posts.CountAsync(p => p.Published),
            SkillCount = await _db.Skills.CountAsync(),
            ProjectCount = await _db.Projects.CountAsync(),
        };

        return View(vm);
    }
}
