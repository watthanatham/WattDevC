using System;
using System.Collections.Generic;

namespace BlogWeb.Models;

public partial class Profile
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Email { get; set; }

    public string? Github { get; set; }

    public string? Linkedin { get; set; }

    public string? Location { get; set; }

    public string? ResumeUrl { get; set; }

    public string? Tagline { get; set; }

    public string? PenAvatarUrl { get; set; }

    public string? PenBio { get; set; }

    public string? PenName { get; set; }
}
