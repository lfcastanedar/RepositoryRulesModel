using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string EncryptedPassword { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordSentAt { get; set; }

    public DateTime? RememberCreatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Jti { get; set; }

    public Guid? RoleId { get; set; }

    public virtual Role? Role { get; set; }
}
