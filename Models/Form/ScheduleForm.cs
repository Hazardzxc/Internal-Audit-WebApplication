namespace STD.Models.Form
{
    public class ScheduleForm
    {
        public int ScheduleID { get; set; }

        public int AuditManagementID { get; set; }

        public string ScheduleDetail { get; set; } = "";

        public string ScheduleDoc { get; set; } = "";

        public DateOnly? ScheduleDate { get; set; }

        public TimeOnly? ScheduleTime { get; set; }
    }
}