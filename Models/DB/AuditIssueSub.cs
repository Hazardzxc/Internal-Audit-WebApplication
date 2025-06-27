using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditIssueSub
{
    /// <summary>
    /// 1 จุดประสงค์ 2 อ้างอิง 3 กิจกรรมการควบคุม
    /// </summary>
    public int AuditIssueSubId { get; set; }

    public int AuditIssueMainId { get; set; }

    public string AuditIssueSubName { get; set; } = null!;

    public int AuditIssueSubType { get; set; }

    public DateTime CreateDate { get; set; }
}
