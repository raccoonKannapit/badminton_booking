using System;
using System.Collections.Generic;

namespace _66088226_Kannapich_Baosri.Models.Db;

public partial class Labstudent
{
    public string StdId { get; set; } = null!;

    public string? StdPassword { get; set; }

    public string? StdName { get; set; }

    public string? StdLastname { get; set; }
}
