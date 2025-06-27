using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditManegementSegmentSubVerify
{
    public Guid AuditManegementSegmentSubVerifyId { get; set; }

    public Guid AuditManegementSegmentSubId { get; set; }

    public Guid AuditIssueActivityId { get; set; }

    public string AuditIssueActivityDetail { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
