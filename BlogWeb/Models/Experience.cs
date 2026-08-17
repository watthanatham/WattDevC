using System;
using System.Collections.Generic;

namespace BlogWeb.Models;

public partial class Experience
{
    public int Id { get; set; }

    public string Company { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Summary { get; set; } = null!;

    public string Highlights { get; set; } = null!;

    public string Tech { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Order { get; set; }
}
