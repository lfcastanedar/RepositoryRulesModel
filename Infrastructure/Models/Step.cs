using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Step
{
    public Guid Id { get; set; }

    public string? Code { get; set; }

    public string? Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Document> Documents { get; } = new List<Document>();
}
