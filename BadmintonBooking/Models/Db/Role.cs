using System;
using System.Collections.Generic;

namespace _66088226_Kannapich_Baosri.Models.Db;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? RoleName { get; set; }

    public int? AccessLevel { get; set; }
}
