using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class AuditManegementSegment
{
    public Guid AuditManegementSegmentId { get; set; }

    public int FacultyId { get; set; }

    public int AuditIssueMainId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime CreateDate { get; set; }
}
