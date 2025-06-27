namespace STD.Models.Form
{
    public class WorkingPaperForm
    {
        public Guid? WorkingPaperID { get; set; }

        public int FacultyID { get; set; }

        public int AuditIssueMainID { get; set; }

        public int No { get; set; }

        public int ReviewNo { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Guid AuditorBy { get; set; }

        public Guid OperationsControllerAuditorBy { get; set; }

        public Guid ProducerBy { get; set; }

        public Guid ReviewerBy { get; set; }

        public Guid ApproveBy { get; set; }

        public DateTime ApproveDate { get; set; }
    }

    public class WorkingPaperFormDraft
    {
        public Guid AuditManegementSegmentID { get; set; }

        public Guid? WorkingPaperID { get; set; }

        public int No { get; set; }

        public int ReviewNo { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Guid AuditorBy { get; set; }

        public Guid OperationsControllerAuditorBy { get; set; }

        public Guid ProducerBy { get; set; }

        public Guid ReviewerBy { get; set; }

        public Guid ApproveBy { get; set; }

        public DateTime ApproveDate { get; set; }
    }
}