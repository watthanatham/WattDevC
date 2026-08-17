using System;
using System.Collections.Generic;

namespace BlogWeb.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string? CoverImage { get; set; }

    public string Category { get; set; } = null!;

    public bool Published { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Excerpt { get; set; }

    public List<string>? Tags { get; set; }
}
