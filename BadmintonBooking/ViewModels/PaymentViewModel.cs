namespace BadmintonBooking.ViewModels;

public class PaymentViewModel
{
    public string UserId { get; set; } = "";
    public string CourtId { get; set; } = "";
    public string BookingDate { get; set; } = "";
    public string SelectedHoursRaw { get; set; } = "";
    public List<int> SelectedHours => SelectedHoursRaw
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).OrderBy(h => h).ToList();
    public decimal BasePrice { get; set; }
    public decimal Price { get; set; }
    public List<AppliedPromotion> AppliedPromotions { get; set; } = new();
}

public class AppliedPromotion
{
    public string Name { get; set; } = "";
    public int Discount { get; set; }
}
