using System;
using System.Collections.Generic;

namespace BadmintonBooking.Models.Db;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? RoleName { get; set; }

    public int? AccessLevel { get; set; }
}
