namespace STD.Models.Form
{
    public class AuditManegementSegmentForm
    {
        public string? AuditManegementSegmentID { get; set; } = string.Empty;

        public int FacultyID { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Item
    {
        public int AuditIssueMainID { get; set; }

        public List<AuditManegementSegmentSubForm> ListAuditManegementSegmentSub { get; set; } = new List<AuditManegementSegmentSubForm>();
    }

    public class AuditManegementSegmentSubForm
    {
        public Guid? AuditManegementSegmentSubID { get; set; }

        public int AuditIssueSubID { get; set; }

        public List<Guid> ListActivityVerify { get; set; } = new List<Guid>();
    }

    public class AuditManegementSegmentEditForm
    {
        public Guid AuditManegementSegmentID { get; set; }

        public int AuditIssueMainID { get; set; }

        public int FacultyID { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public List<AuditManegementSegmentSubForm> Items { get; set; } = new List<AuditManegementSegmentSubForm>();
    }
}