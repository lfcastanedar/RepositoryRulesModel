using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Permission
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Code { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; } = new List<RolePermission>();
}
