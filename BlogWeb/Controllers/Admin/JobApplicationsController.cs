using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Data;
using BlogWeb.Models;

namespace BlogWeb.Controllers.Admin;

/// <summary>บันทึกการสมัครงาน — CRUD for the job application tracker.</summary>
[Authorize]
[Route("admin/jobs")]
public class JobApplicationsController : Controller
{
    private readonly BlogDbContext _db;

    public JobApplicationsController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        return View(new JobApplicationIndexViewModel { Applications = await LoadAsync() });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobApplicationFormViewModel form)
    {
        var (data, error) = ReadAndValidate(form);
        if (error != null)
        {
            return View("Index", new JobApplicationIndexViewModel { Applications = await LoadAsync(), Error = error, Draft = data });
        }

        _db.JobApplications.Add(data);
        await _db.SaveChangesAsync();
        return Redirect("/admin/jobs");
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, JobApplicationFormViewModel form)
    {
        var (data, error) = ReadAndValidate(form);
        if (error != null)
        {
            return View("Index", new JobApplicationIndexViewModel { Applications = await LoadAsync(), Error = error, ErrorId = id, Draft = data });
        }

        var existing = await _db.JobApplications.FindAsync(id);
        if (existing == null) return Redirect("/admin/jobs");

        existing.Company = data.Company;
        existing.Position = data.Position;
        existing.AppliedDate = data.AppliedDate;
        existing.Status = data.Status;
        existing.Link = data.Link;
        existing.Reason = data.Reason;
        await _db.SaveChangesAsync();

        return Redirect("/admin/jobs");
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _db.JobApplications.FindAsync(id);
        if (application != null)
        {
            _db.JobApplications.Remove(application);
            await _db.SaveChangesAsync();
        }
        return Redirect("/admin/jobs");
    }

    private Task<List<JobApplication>> LoadAsync() =>
        _db.JobApplications.OrderByDescending(j => j.AppliedDate).ThenByDescending(j => j.Id).ToListAsync();

    private static (JobApplication Data, string? Error) ReadAndValidate(JobApplicationFormViewModel form)
    {
        string? Optional(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        var company = (form.Company ?? "").Trim();
        var position = (form.Position ?? "").Trim();
        var status = JobApplicationStatus.IsValid(form.Status) ? form.Status : "APPLIED";
        DateOnly.TryParse(form.AppliedDate, System.Globalization.CultureInfo.InvariantCulture, out var appliedDate);

        var data = new JobApplication
        {
            Company = company,
            Position = position,
            AppliedDate = appliedDate,
            Status = status,
            Link = Optional(form.Link),
            Reason = Optional(form.Reason),
        };

        if (company.Length == 0 || position.Length == 0) return (data, "กรุณากรอกบริษัทและตำแหน่งที่สมัคร");
        if (appliedDate == default) return (data, "กรุณาระบุวันที่สมัคร");

        return (data, null);
    }
}
