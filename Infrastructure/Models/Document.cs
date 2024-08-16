using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Document
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? StepId { get; set; }

    public Guid? ModelPageId { get; set; }

    public string? DocumentNumber { get; set; }

    public virtual ModelPage? ModelPage { get; set; }

    public virtual Step? Step { get; set; }
}
