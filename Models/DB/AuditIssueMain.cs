using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditIssueMain
{
    public int AuditIssueMainId { get; set; }

    public string AuditIssueMainName { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
