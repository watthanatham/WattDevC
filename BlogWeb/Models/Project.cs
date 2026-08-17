using System;
using System.Collections.Generic;

namespace BlogWeb.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Link { get; set; }

    public int Order { get; set; }

    public string? Problem { get; set; }

    public string? Result { get; set; }

    public string? Solution { get; set; }
}
