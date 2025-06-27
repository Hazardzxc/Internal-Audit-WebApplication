using System;
using System.Collections.Generic;

namespace STD.Models.DB;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string FacultyName { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
