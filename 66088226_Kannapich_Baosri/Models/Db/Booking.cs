using System;
using System.Collections.Generic;

namespace _66088226_Kannapich_Baosri.Models.Db;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string? UserId { get; set; }

    public DateOnly? BookDate { get; set; }

    public decimal? Price { get; set; }

    public string? CourtId { get; set; }
}
