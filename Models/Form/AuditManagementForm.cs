namespace STD.Models.Form
{
    public class AuditManagementForm
    {
        public int AuditManagementID { get; set; }

        public int FacultyID { get; set; }

        public int AuditIssueMainID { get; set; }

        public int Status { get; set; }

        public List<int> AuditIssueSubID { get; set; } = new List<int>();

        public DateOnly? AuditStartDate { get; set; }

        public DateOnly? AuditEndDate { get; set; }
    }
}