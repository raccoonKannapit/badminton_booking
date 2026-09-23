namespace BadmintonBooking.ViewModels
{
    public class LabUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CourtsViewModel
    {
        public string CourtId { get; set; }
        public string CourtName { get; set; }
    }

    public class BookingViewModel
    {
        public string BookingId { get; set; }
        public string UserId { get; set; }
        public string CourtId { get; set; }
        public DateOnly BookingDate { get; set; }
        public string SelectedHoursRaw { get; set; } = "";
        public decimal Price { get; set; }
        public List<int> TakenHours { get; set; } = new();
        public List<string> Slots { get; set; } = new();
    }

    public class EditBookingViewModel
    {
        public string BookingId { get; set; }
        public string CourtId { get; set; }
        public DateOnly BookingDate { get; set; }
        public string SelectedHoursRaw { get; set; } = "";
        public List<int> TakenHours { get; set; } = new();
        public List<int> CurrentHours { get; set; } = new();
        public decimal Price { get; set; }
        public List<CourtsViewModel> Courts { get; set; } = new();
    }

    public class DashboardViewModel
    {
        public int TodayBookings { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCourts { get; set; }
        public List<CourtOccupancy> CourtOccupancy { get; set; } = new();
        public List<RecentBooking> RecentBookings { get; set; } = new();
    }

    public class CourtOccupancy
    {
        public string CourtId { get; set; } = "";
        public string CourtName { get; set; } = "";
        public List<string> BookedSlots { get; set; } = new();
    }

    public class RecentBooking
    {
        public string BookingId { get; set; } = "";
        public string UserId { get; set; } = "";
        public string CourtId { get; set; } = "";
        public DateOnly BookingDate { get; set; }
        public decimal Price { get; set; }
        public List<string> Slots { get; set; } = new();
    }

    public class UserDataViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}