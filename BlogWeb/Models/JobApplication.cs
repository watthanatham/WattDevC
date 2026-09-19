namespace BlogWeb.Models;

public partial class JobApplication
{
    public int Id { get; set; }

    public string Company { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateOnly AppliedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Link { get; set; }

    public string? Reason { get; set; }
}

/// <summary>The status options — shared by the dropdown, the badge and server-side validation.</summary>
public static class JobApplicationStatus
{
    public static readonly (string Value, string Label, string Color)[] All =
    {
        ("APPLIED", "ส่งใบสมัครแล้ว", "#6366f1"),
        ("INTERVIEW", "นัดสัมภาษณ์", "#f59e0b"),
        ("OFFER", "ได้ข้อเสนอ", "#10b981"),
        ("Waiting for interview result", "รอผลสัมภาษณ์", "#fb8b24"),
        ("Waiting for interview", "รอสัมภาษณ์", "#ffba08"),
        ("ACCEPTED", "รับข้อเสนอแล้ว", "#059669"),
        ("REJECTED", "ไม่ผ่าน", "#ef4444"),
        ("No contact", "ไม่มีการติดต่อ", "#e5e5e5"),
    };

    public static bool IsValid(string? value) => All.Any(s => s.Value == value);

    public static (string Label, string Color) Display(string value)
    {
        var match = All.FirstOrDefault(s => s.Value == value);
        return match.Value == null ? (value, "#71717a") : (match.Label, match.Color);
    }
}
