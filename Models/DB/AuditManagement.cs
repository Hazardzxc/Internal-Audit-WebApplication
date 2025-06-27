using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditManagement
{
    public int AuditManagementId { get; set; }

    public int FacultyId { get; set; }

    public int AuditIssueMainId { get; set; }

    public DateOnly AuditStartDate { get; set; }

    public DateOnly AuditEndDate { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }
}
