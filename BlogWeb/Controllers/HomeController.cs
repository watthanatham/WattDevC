using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;

namespace BlogWeb.Controllers;

public class HomeController : Controller
{
    private readonly BlogDbContext _db;

    public HomeController(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Id == 1);
        var skills = await _db.Skills.OrderBy(s => s.Order).ToListAsync();
        var projects = await _db.Projects.OrderBy(p => p.Order).ToListAsync();
        var experiences = await _db.Experiences.OrderByDescending(e => e.StartDate).ToListAsync();
        var latestPosts = await _db.Posts
            .Where(p => p.Published)
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        var vm = new HomeViewModel
        {
            Profile = profile,
            Skills = skills,
            Projects = projects,
            Experiences = experiences,
            LatestPosts = latestPosts,
            Years = CalcYears(experiences),
            // A project renders as a case study / "Boss Battle" only when the full
            // problem → solution → result story is filled in.
            CaseStudies = projects
                .Where(p => !string.IsNullOrEmpty(p.Problem) && !string.IsNullOrEmpty(p.Solution) && !string.IsNullOrEmpty(p.Result))
                .ToList(),
        };

        return View(vm);
    }

    /// <summary>Years of experience, derived from the earliest WORK/FREELANCE entry.</summary>
    private static int CalcYears(List<Experience> experiences)
    {
        var professional = experiences.Where(e => e.Type != "EDUCATION").ToList();
        if (professional.Count == 0) return 0;

        var earliest = professional.Min(e => e.StartDate);
        var years = (DateTime.UtcNow - earliest).TotalDays / 365.25;
        return Math.Max(1, (int)Math.Floor(years));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
