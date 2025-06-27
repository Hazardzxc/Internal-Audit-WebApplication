using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int AuditManagementId { get; set; }

    public string ScheduleDetail { get; set; } = null!;

    public string ScheduleDoc { get; set; } = null!;

    public DateOnly ScheduleDate { get; set; }

    public TimeOnly ScheduleTime { get; set; }

    public DateTime CreateData { get; set; }
}
