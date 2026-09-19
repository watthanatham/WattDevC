namespace BlogWeb.Models;

public class JobApplicationIndexViewModel
{
    public List<JobApplication> Applications { get; set; } = new();

    public string? Error { get; set; }

    /// <summary>When an edit fails validation, re-open the modal on that row. Null = the add form.</summary>
    public int? ErrorId { get; set; }

    /// <summary>The rejected submission, so the re-opened modal keeps what was typed.</summary>
    public JobApplication? Draft { get; set; }
}

public class JobApplicationFormViewModel
{
    public string Company { get; set; } = "";
    public string Position { get; set; } = "";
    /// <summary>"yyyy-MM-dd" from &lt;input type="date"&gt;.</summary>
    public string AppliedDate { get; set; } = "";
    public string Status { get; set; } = "APPLIED";
    public string Link { get; set; } = "";
    public string Reason { get; set; } = "";
}
