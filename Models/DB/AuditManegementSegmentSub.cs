using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditManegementSegmentSub
{
    public Guid AuditManegementSegmentSubId { get; set; }

    public Guid AuditManegementSegmentId { get; set; }

    public int AuditIssueSubId { get; set; }

    public int AuditIssueSubType { get; set; }

    public string AuditIssueSubDetail { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
