using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditIssueActivityVerify
{
    public Guid AuditIssueActivityId { get; set; }

    public int AuditIssueSubId { get; set; }

    public string ActivityDetail { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
