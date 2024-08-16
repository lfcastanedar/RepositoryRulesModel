using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class RolePermission
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? RoleId { get; set; }

    public Guid? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }

    public virtual Role? Role { get; set; }
}
