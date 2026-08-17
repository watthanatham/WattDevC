namespace BlogWeb.Models;

/// <summary>Everything the home page needs, computed once in the controller — mirrors src/app/(site)/page.tsx.</summary>
public class HomeViewModel
{
    public Profile? Profile { get; set; }
    public List<Skill> Skills { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<Experience> Experiences { get; set; } = new();
    public List<Project> CaseStudies { get; set; } = new();
    public List<Post> LatestPosts { get; set; } = new();
    public int Years { get; set; }
}
