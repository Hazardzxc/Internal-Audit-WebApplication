using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class WorkingPaper
{
    public Guid WorkingPaperId { get; set; }

    public int No { get; set; }

    public int FacultyId { get; set; }

    public int AuditIssueMainId { get; set; }

    public Guid AuditorBy { get; set; }

    public Guid OperationsControllerAuditorBy { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int ReviewNo { get; set; }

    /// <summary>
    /// ผู้จัดทำ
    /// </summary>
    public Guid ProducerBy { get; set; }

    /// <summary>
    /// ผู้สอบทาน
    /// </summary>
    public Guid ReviewerBy { get; set; }

    public Guid ApproveBy { get; set; }

    public DateTime ApproveDate { get; set; }

    public Guid? AuditManegementSegmentId { get; set; }

    public DateTime CreateDate { get; set; }
}
