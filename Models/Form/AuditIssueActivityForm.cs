namespace STD.Models.Form
{
    public class AuditIssueActivityVerifyForm
    {
        public Guid? AuditIssueActivityID { get; set; }
        public int AuditIssueSubID { get; set; } = 0;
        public string ActivityDetail { get; set; } = "";
    }
}