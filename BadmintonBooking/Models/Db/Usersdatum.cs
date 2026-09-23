using System;
using System.Collections.Generic;

namespace BadmintonBooking.Models.Db;

public partial class Usersdatum
{
    public string UserId { get; set; } = null!;

    public string? Username { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public bool IsMember { get; set; }
}
