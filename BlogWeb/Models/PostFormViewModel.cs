namespace BlogWeb.Models;

public class PostFormViewModel
{
    public int? Id { get; set; }
    public string Title { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Category { get; set; } = "Street";
    public string Tags { get; set; } = "";
    public bool Published { get; set; }
    public string CoverImage { get; set; } = "";
    public string Body { get; set; } = "";
    public string? Error { get; set; }
}
