using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class WorkingPaperActivity
{
    public Guid WorkingPaperActivityId { get; set; }

    public Guid WorkingPaperId { get; set; }

    public int AuditIssueSubId { get; set; }

    /// <summary>
    /// 1 เป็นประเด็น 2 ไม่เป็นประเด็น
    /// </summary>
    public int Issue { get; set; }

    public string? InspectionResults { get; set; }

    public string? Notes { get; set; }

    public DateTime CreateDate { get; set; }
}
