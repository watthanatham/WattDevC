using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;

namespace BlogWeb.Controllers.Admin;

/// <summary>Profile/Skills/Experience/Projects CRUD — ported from portfolio.ts + experience.ts.</summary>
[Authorize]
[Route("admin/portfolio")]
public class PortfolioController : Controller
{
    private static readonly string[] ValidExperienceTypes = { "WORK", "FREELANCE", "EDUCATION" };
    private readonly BlogDbContext _db;

    public PortfolioController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var vm = await BuildIndexViewModelAsync();
        return View(vm);
    }

    private async Task<PortfolioIndexViewModel> BuildIndexViewModelAsync() => new()
    {
        Profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Id == 1),
        Skills = await _db.Skills.OrderBy(s => s.Order).ToListAsync(),
        Projects = await _db.Projects.OrderBy(p => p.Order).ToListAsync(),
        Experiences = await _db.Experiences.OrderByDescending(e => e.StartDate).ToListAsync(),
    };

    // ---------------------------------------------------------------- Profile

    [HttpPost("profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileFormViewModel form)
    {
        string? Optional(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        var name = (form.Name ?? "").Trim();
        var role = (form.Role ?? "").Trim();
        var bio = (form.Bio ?? "").Trim();

        if (name.Length == 0 || role.Length == 0 || bio.Length == 0)
        {
            var vm = await BuildIndexViewModelAsync();
            vm.ProfileError = "กรุณากรอกข้อมูลให้ครบ";
            return View("Index", vm);
        }

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Id == 1) ?? new Profile { Id = 1 };
        var isNew = _db.Entry(profile).State == EntityState.Detached;

        profile.Name = name;
        profile.Role = role;
        profile.Bio = bio;
        profile.Tagline = Optional(form.Tagline);
        profile.Location = Optional(form.Location);
        profile.Email = Optional(form.Email);
        profile.Github = Optional(form.Github);
        profile.Linkedin = Optional(form.Linkedin);
        profile.PenName = Optional(form.PenName);
        profile.PenBio = Optional(form.PenBio);

        // Uploaded in the browser; only overwrite when a new file was uploaded.
        if (Optional(form.AvatarUrl) is { } avatarUrl) profile.AvatarUrl = avatarUrl;
        if (Optional(form.ResumeUrl) is { } resumeUrl) profile.ResumeUrl = resumeUrl;
        if (Optional(form.PenAvatarUrl) is { } penAvatarUrl) profile.PenAvatarUrl = penAvatarUrl;

        if (isNew) _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        var result = await BuildIndexViewModelAsync();
        result.ProfileSuccess = true;
        return View("Index", result);
    }

    // ----------------------------------------------------------------- Skills

    [HttpPost("skills")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSkill(SkillFormViewModel form)
    {
        var name = (form.Name ?? "").Trim();
        var iconUrl = (form.IconUrl ?? "").Trim();

        if (name.Length == 0 || iconUrl.Length == 0)
        {
            var vm = await BuildIndexViewModelAsync();
            vm.SkillError = name.Length == 0 ? "กรุณากรอกชื่อ skill" : "กรุณาเลือกไฟล์ไอคอน";
            return View("Index", vm);
        }

        var lastOrder = await _db.Skills.OrderByDescending(s => s.Order).Select(s => (int?)s.Order).FirstOrDefaultAsync();
        _db.Skills.Add(new Skill { Name = name, IconUrl = iconUrl, Category = form.Category ?? "other", Order = (lastOrder ?? 0) + 1 });
        await _db.SaveChangesAsync();

        return Redirect("/admin/portfolio#skills");
    }

    [HttpPost("skills/{id:int}/icon")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSkillIcon(int id, string iconUrl)
    {
        var skill = await _db.Skills.FindAsync(id);
        if (skill != null && !string.IsNullOrWhiteSpace(iconUrl))
        {
            skill.IconUrl = iconUrl.Trim();
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/portfolio#skills");
    }

    [HttpPost("skills/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _db.Skills.FindAsync(id);
        if (skill != null)
        {
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/portfolio#skills");
    }

    // --------------------------------------------------------------- Projects

    [HttpPost("projects")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProject(ProjectFormViewModel form)
    {
        string? Optional(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        var title = (form.Title ?? "").Trim();
        var description = (form.Description ?? "").Trim();

        if (title.Length == 0 || description.Length == 0)
        {
            var vm = await BuildIndexViewModelAsync();
            vm.ProjectError = "กรุณากรอกชื่อและคำอธิบายโปรเจค";
            return View("Index", vm);
        }

        var lastOrder = await _db.Projects.OrderByDescending(p => p.Order).Select(p => (int?)p.Order).FirstOrDefaultAsync();
        _db.Projects.Add(new Project
        {
            Title = title,
            Description = description,
            Link = Optional(form.Link),
            ImageUrl = Optional(form.ImageUrl),
            Problem = Optional(form.Problem),
            Solution = Optional(form.Solution),
            Result = Optional(form.Result),
            Order = (lastOrder ?? 0) + 1,
        });
        await _db.SaveChangesAsync();

        return Redirect("/admin/portfolio#projects");
    }

    [HttpPost("projects/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project != null)
        {
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/portfolio#projects");
    }

    // ------------------------------------------------------------ Experience

    [HttpPost("experience")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExperience(ExperienceFormViewModel form)
    {
        var (data, error) = ReadAndValidateExperience(form);
        if (error != null)
        {
            var vm = await BuildIndexViewModelAsync();
            vm.ExperienceError = error;
            return View("Index", vm);
        }

        _db.Experiences.Add(new Experience
        {
            Company = data.Company,
            Role = data.Role,
            Summary = data.Summary,
            Highlights = data.Highlights,
            Tech = data.Tech,
            Type = data.Type,
            StartDate = data.StartDate!.Value,
            EndDate = data.EndDate,
        });
        await _db.SaveChangesAsync();

        return Redirect("/admin/portfolio#experience");
    }

    [HttpPost("experience/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateExperience(int id, ExperienceFormViewModel form)
    {
        var (data, error) = ReadAndValidateExperience(form);
        if (error != null)
        {
            var vm = await BuildIndexViewModelAsync();
            vm.ExperienceError = error;
            vm.ExperienceErrorId = id;
            return View("Index", vm);
        }

        var existing = await _db.Experiences.FindAsync(id);
        if (existing == null) return Redirect("/admin/portfolio#experience");

        existing.Company = data.Company;
        existing.Role = data.Role;
        existing.Summary = data.Summary;
        existing.Highlights = data.Highlights;
        existing.Tech = data.Tech;
        existing.Type = data.Type;
        existing.StartDate = data.StartDate!.Value;
        existing.EndDate = data.EndDate;
        await _db.SaveChangesAsync();

        return Redirect("/admin/portfolio#experience");
    }

    [HttpPost("experience/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExperience(int id)
    {
        var experience = await _db.Experiences.FindAsync(id);
        if (experience != null)
        {
            _db.Experiences.Remove(experience);
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/portfolio#experience");
    }

    private record ExperienceData(string Company, string Role, string Summary, string Highlights, string Tech, string Type, DateTime? StartDate, DateTime? EndDate);

    /// <summary>`&lt;input type="month"&gt;` gives "YYYY-MM"; anchor it to the 1st of the month.</summary>
    private static DateTime? ParseMonth(string? value) =>
        // The "timestamp without time zone" column expects Kind=Unspecified — Npgsql
        // rejects Utc-kinded values for it.
        !string.IsNullOrEmpty(value) && DateTime.TryParseExact(value, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var date)
            ? DateTime.SpecifyKind(date, DateTimeKind.Unspecified)
            : null;

    private static (ExperienceData Data, string? Error) ReadAndValidateExperience(ExperienceFormViewModel form)
    {
        var company = (form.Company ?? "").Trim();
        var role = (form.Role ?? "").Trim();
        var summary = (form.Summary ?? "").Trim();
        var highlights = (form.Highlights ?? "").Trim();
        var tech = (form.Tech ?? "").Trim();
        var type = ValidExperienceTypes.Contains(form.Type) ? form.Type : "WORK";
        var startDate = ParseMonth(form.StartDate);
        var endDate = form.IsCurrent ? null : ParseMonth(form.EndDate);
        var data = new ExperienceData(company, role, summary, highlights, tech, type, startDate, endDate);

        if (company.Length == 0 || role.Length == 0) return (data, "กรุณากรอกบริษัทและตำแหน่ง");
        // A role described purely as a bullet list is fine — only require that at
        // least one of the two description fields has content.
        if (summary.Length == 0 && highlights.Length == 0) return (data, "กรุณากรอกสรุป หรือผลงานเด่น อย่างน้อยหนึ่งอย่าง");
        if (startDate == null) return (data, "กรุณาระบุเดือน/ปีที่เริ่มงาน");
        if (!form.IsCurrent && endDate == null) return (data, "กรุณาระบุเดือน/ปีที่สิ้นสุด หรือติ๊ก 'ทำอยู่ปัจจุบัน'");
        if (endDate != null && endDate < startDate) return (data, "วันที่สิ้นสุดต้องอยู่หลังวันที่เริ่ม");

        return (data, null);
    }
}
