namespace BlogWeb.Models;

public class PortfolioIndexViewModel
{
    public Profile? Profile { get; set; }
    public List<Skill> Skills { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<Experience> Experiences { get; set; } = new();

    public string? ProfileError { get; set; }
    public bool ProfileSuccess { get; set; }
    public string? SkillError { get; set; }
    public string? ProjectError { get; set; }
    public string? ExperienceError { get; set; }
    /// <summary>When an experience edit fails validation, re-show that row's form with the error.</summary>
    public int? ExperienceErrorId { get; set; }
}

public class ProfileFormViewModel
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Bio { get; set; } = "";
    public string Tagline { get; set; } = "";
    public string Location { get; set; } = "";
    public string Email { get; set; } = "";
    public string Github { get; set; } = "";
    public string Linkedin { get; set; } = "";
    public string AvatarUrl { get; set; } = "";
    public string ResumeUrl { get; set; } = "";
    public string PenName { get; set; } = "";
    public string PenBio { get; set; } = "";
    public string PenAvatarUrl { get; set; } = "";
}

public class SkillFormViewModel
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "frontend";
    public string IconUrl { get; set; } = "";
}

public class ProjectFormViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Link { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string Problem { get; set; } = "";
    public string Solution { get; set; } = "";
    public string Result { get; set; } = "";
}

public class ExperienceFormViewModel
{
    public int? Id { get; set; }
    public string Company { get; set; } = "";
    public string Role { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Highlights { get; set; } = "";
    public string Tech { get; set; } = "";
    public string Type { get; set; } = "WORK";
    public bool IsCurrent { get; set; }
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
}
