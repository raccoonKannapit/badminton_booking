using System;
using System.Collections.Generic;

namespace BadmintonBooking.Models.Db;

public partial class Court
{
    public string CourtId { get; set; } = null!;

    public string? CourtName { get; set; }
}
