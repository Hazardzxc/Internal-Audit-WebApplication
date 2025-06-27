namespace STD.Models.Form
{
    public class AuditIssueMainForm
    {
        public int AuditIssueMainID { get; set; } = 0;
        public string AuditIssueMainName { get; set; } = "";
        public int FacultyID { get; set; } = 0;
        public List<AuditIssueSubForm> ListAuditIssueSub { get; set; } = new List<AuditIssueSubForm>();
    }
}