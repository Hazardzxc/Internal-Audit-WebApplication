using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class Auditor
{
    public int AuditorId { get; set; }

    public string AuditorName { get; set; } = null!;
}
