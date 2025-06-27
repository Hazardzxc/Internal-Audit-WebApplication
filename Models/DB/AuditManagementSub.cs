using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditManagementSub
{
    public int AuditManagementSubId { get; set; }

    public int AuditManagementId { get; set; }

    public int AuditIssueSubId { get; set; }

    public DateTime CreateDate { get; set; }
}
