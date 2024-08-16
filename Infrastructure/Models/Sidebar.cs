using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Sidebar
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Icon { get; set; }

    public string? Path { get; set; }

    public Guid? SidebarId { get; set; }

    public int? Order { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
