using System;
using System.Collections.Generic;

namespace BlogWeb.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string IconUrl { get; set; } = null!;

    public int Order { get; set; }

    public string Category { get; set; } = null!;

    public int Level { get; set; }
}
