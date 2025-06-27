namespace STD.Models.Form
{
    public class WorkingPaperActivityForm
    {
        public Guid? WorkingPaperActivityID { get; set; }
        public Guid WorkingPaperID { get; set; }
        public int AuditIssueSubID { get; set; }
    }
}