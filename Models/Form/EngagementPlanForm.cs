namespace STD.Models.Form
{
    public class EngagementPlanForm
    {
        public int EngagementPlanID { get; set; }

        public int AuditManagementID { get; set; }

        public Guid AuditorID { get; set; }

        public string AuditProcedure { get; set; } = "";

        public string WPCode { get; set; } = "";

        public TimeOnly? AuditTimeStart { get; set; }

        public TimeOnly? AuditTimeEnd { get; set; }
    }
}