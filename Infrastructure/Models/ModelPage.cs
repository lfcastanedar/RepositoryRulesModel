using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ModelPage
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Code { get; set; }

    public virtual ICollection<Document> Documents { get; } = new List<Document>();
}
