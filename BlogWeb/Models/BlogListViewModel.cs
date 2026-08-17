namespace BlogWeb.Models;

public class BlogListViewModel
{
    public List<Post> Posts { get; set; } = new();
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public string Search { get; set; } = "";
    public string Category { get; set; } = "";
    public string Author { get; set; } = "Journal";
    public string? PenBio { get; set; }
}

public class BlogDetailViewModel
{
    public Post Post { get; set; } = null!;
    public string Author { get; set; } = "Anonymous";
    public string? PenAvatarUrl { get; set; }
    public int ReadingMinutes { get; set; }
    public List<Post> Related { get; set; } = new();
}
