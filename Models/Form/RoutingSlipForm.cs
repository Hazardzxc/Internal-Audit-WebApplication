namespace STD.Models.Form
{
    public class RoutingSlipForm
    {
        public int RoutingSlipID { get; set; }

        public int AuditManagementID { get; set; }

        public string RoutingSlipDetail { get; set; } = "";

        public string AuditPlan { get; set; } = "";

        public string Practicality { get; set; } = "";

        public string Comment { get; set; } = "";
    }
}