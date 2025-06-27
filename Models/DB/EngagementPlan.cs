using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class EngagementPlan
{
    public int EngagementPlanId { get; set; }

    public int AuditManagementId { get; set; }

    public Guid AuditorId { get; set; }

    public TimeOnly AuditTimeStart { get; set; }

    public TimeOnly AuditTimeEnd { get; set; }

    public string AuditProcedure { get; set; } = null!;

    public string? Wpcode { get; set; }

    public DateTime CreateDate { get; set; }
}
