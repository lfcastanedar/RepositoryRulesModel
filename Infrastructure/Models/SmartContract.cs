using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class SmartContract
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Version { get; set; }

    public bool? Active { get; set; }

    public string? Type { get; set; }

    public string? Source { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
