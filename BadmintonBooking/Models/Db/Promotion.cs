using System;
using System.Collections.Generic;

namespace BadmintonBooking.Models.Db;

public partial class Promotion
{
    public string PromotionId { get; set; } = null!;

    public string? PromotionName { get; set; }

    public string? ConditionType { get; set; }

    public int? ConditionValue { get; set; }

    public int? DiscountValue { get; set; }
}
