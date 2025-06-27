using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class RoutingSlip
{
    public int RoutingSlipId { get; set; }

    public int AuditManagementId { get; set; }

    public string RoutingSlipDetail { get; set; } = null!;

    public string AuditPlan { get; set; } = null!;

    public string Practicality { get; set; } = null!;

    public string? Comment { get; set; }

    public DateTime CreateDate { get; set; }
}
