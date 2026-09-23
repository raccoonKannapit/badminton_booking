using System;
using System.Collections.Generic;

namespace BadmintonBooking.Models.Db;

public partial class BookTime
{
    public string BtId { get; set; } = null!;

    public string? BookingId { get; set; }

    public string? CourtId { get; set; }

    public string? Booktimeslot { get; set; }

    public DateOnly? BookDate { get; set; }
}
