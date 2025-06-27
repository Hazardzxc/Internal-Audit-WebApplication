using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class UserAuditor
{
    public Guid AuditorId { get; set; }

    public string AuditorCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    /// <summary>
    /// 1 Auditor 2 Director 99 Admin
    /// </summary>
    public int Role { get; set; }

    public DateTime CreateDate { get; set; }
}
