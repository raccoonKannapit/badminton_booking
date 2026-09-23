using Microsoft.AspNetCore.Mvc;
using _66088226_Kannapich_Baosri.Models.Db;
using _66088226_Kannapich_Baosri.ViewModels;

namespace _66088226_Kannapich_Baosri.Controllers;

public class AdminController : Controller
{
    private readonly Csi402dbContext _db;

    public AdminController(Csi402dbContext db)
    {
        _db = db;
    }

    public IActionResult UserManagement()
    {
        var users = _db.Usersdata.ToList();
        return View(users);
    }

    public IActionResult EditUser(string UID)
    {
        var user = (from u in _db.Usersdata where u.UserId == UID select new UserDataViewModel
        {
            UserId   = u.UserId,
            Username = u.Username,
            Phone    = u.Phone,
            Email    = u.Email,
            Role     = u.Role,
            Password = u.Password
        }).FirstOrDefault();
        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(UserDataViewModel data)
    {
        var user = _db.Usersdata.FirstOrDefault(u => u.UserId == data.UserId);
        user.Username = data.Username;
        user.Phone    = data.Phone;
        user.Email    = data.Email;
        user.Role     = data.Role;
        user.Password = data.Password;
        _db.Update(user);
        _db.SaveChanges();
        return RedirectToAction("UserManagement");
    }

    public IActionResult DeleteUser(string UID)
    {
        var user = _db.Usersdata.FirstOrDefault(u => u.UserId == UID);
        if (user != null)
        {
            _db.Remove(user);
            _db.SaveChanges();
        }
        return RedirectToAction("UserManagement");
    }

    public IActionResult Dashboard()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var todayBookings = _db.Bookings.Where(b => b.BookDate == today).ToList();
        var todayRevenue  = todayBookings.Sum(b => b.Price ?? 0);

        var allCourts = _db.Courts.ToList();
        var todaySlots = _db.BookTimes.Where(bt => bt.BookDate == today).ToList();

        var courtOccupancy = allCourts.Select(c => new CourtOccupancy
        {
            CourtId    = c.CourtId,
            CourtName  = c.CourtName ?? c.CourtId,
            BookedSlots = todaySlots
                .Where(bt => bt.CourtId == c.CourtId && bt.Booktimeslot != null)
                .Select(bt => bt.Booktimeslot!)
                .OrderBy(s => s)
                .ToList()
        }).ToList();

        var recentBookingRecords = _db.Bookings
            .OrderByDescending(b => b.BookingId)
            .Take(8)
            .ToList();

        var recentIds = recentBookingRecords.Select(b => b.BookingId).ToList();
        var recentSlots = _db.BookTimes
            .Where(bt => bt.BookingId != null && recentIds.Contains(bt.BookingId))
            .ToList()
            .GroupBy(bt => bt.BookingId!)
            .ToDictionary(g => g.Key, g => g.Select(bt => bt.Booktimeslot!).OrderBy(s => s).ToList());

        var recentBookings = recentBookingRecords.Select(b => new RecentBooking
        {
            BookingId   = b.BookingId,
            UserId      = b.UserId ?? "",
            CourtId     = b.CourtId ?? "",
            BookingDate = b.BookDate ?? today,
            Price       = b.Price ?? 0,
            Slots       = recentSlots.TryGetValue(b.BookingId, out var s) ? s : []
        }).ToList();

        var model = new DashboardViewModel
        {
            TodayBookings  = todayBookings.Count,
            TodayRevenue   = todayRevenue,
            TotalUsers     = _db.Usersdata.Count(),
            TotalCourts    = allCourts.Count,
            CourtOccupancy = courtOccupancy,
            RecentBookings = recentBookings
        };

        return View(model);
    }

    private const string WalkInUserId = "WALKIN";

    public IActionResult WalkInBooking(string? courtId = null, string? date = null)
    {
        var courts = _db.Courts
            .Select(c => new CourtsViewModel { CourtId = c.CourtId, CourtName = c.CourtName })
            .ToList();

        courtId ??= courts.FirstOrDefault()?.CourtId ?? "";

        var bookDateVal = date != null
            ? DateOnly.Parse(date)
            : DateOnly.FromDateTime(DateTime.Today);

        var takenHours = _db.BookTimes
            .Where(bt => bt.CourtId == courtId && bt.BookDate == bookDateVal)
            .ToList()
            .Where(bt => bt.Booktimeslot != null)
            .Select(bt => int.Parse(bt.Booktimeslot!.Split(':')[0]))
            .Distinct()
            .ToList();

        var model = new EditBookingViewModel
        {
            BookingId        = "",
            CourtId          = courtId,
            BookingDate      = bookDateVal,
            TakenHours       = takenHours,
            CurrentHours     = new(),
            SelectedHoursRaw = "",
            Courts           = courts
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult WalkInBooking(EditBookingViewModel data)
    {
        var bookingId = Guid.NewGuid().ToString("N")[..8].ToUpper();

        var booking = new Booking
        {
            BookingId = bookingId,
            UserId    = WalkInUserId,
            CourtId   = data.CourtId,
            BookDate  = data.BookingDate,
            Price     = data.SelectedHoursRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries).Length * 200m
        };
        _db.Add(booking);

        foreach (var h in data.SelectedHoursRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse))
        {
            _db.Add(new BookTime
            {
                BtId         = Guid.NewGuid().ToString(),
                BookingId    = bookingId,
                CourtId      = data.CourtId,
                Booktimeslot = h.ToString("D2") + ":00",
                BookDate     = data.BookingDate
            });
        }

        _db.SaveChanges();
        return RedirectToAction("Bookings");
    }

    public IActionResult Bookings()
    {
        var bookings   = _db.Bookings.ToList();
        var bookingIds = bookings.Select(b => b.BookingId).ToList();

        var slotsByBooking = _db.BookTimes
            .Where(bt => bt.BookingId != null && bookingIds.Contains(bt.BookingId))
            .ToList()
            .GroupBy(bt => bt.BookingId!)
            .ToDictionary(g => g.Key, g => g.Select(bt => bt.Booktimeslot!).OrderBy(s => s).ToList());

        var models = bookings.Select(b => new BookingViewModel
        {
            BookingId   = b.BookingId,
            UserId      = b.UserId ?? "",
            CourtId     = b.CourtId ?? "",
            BookingDate = b.BookDate ?? DateOnly.FromDateTime(DateTime.Now),
            Price       = b.Price ?? 0,
            Slots       = slotsByBooking.TryGetValue(b.BookingId, out var s) ? s : []
        }).ToList();

        return View(models);
    }

    public IActionResult EditBooking(string bookingId, string? date = null, string? courtId = null)
    {
        var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return RedirectToAction("Bookings");

        var bookDateVal = date != null
            ? DateOnly.Parse(date)
            : booking.BookDate ?? DateOnly.FromDateTime(DateTime.Today);

        courtId ??= booking.CourtId;

        var currentHours = _db.BookTimes
            .Where(bt => bt.BookingId == bookingId)
            .Select(bt => bt.Booktimeslot)
            .ToList()
            .Where(s => s != null)
            .Select(s => int.Parse(s!.Split(':')[0]))
            .ToList();

        var takenHours = _db.BookTimes
            .Where(bt => bt.CourtId == courtId && bt.BookDate == bookDateVal)
            .ToList()
            .Where(bt => bt.BookingId != bookingId && bt.Booktimeslot != null)
            .Select(bt => int.Parse(bt.Booktimeslot!.Split(':')[0]))
            .Distinct()
            .ToList();

        var courts = _db.Courts
            .Select(c => new CourtsViewModel { CourtId = c.CourtId, CourtName = c.CourtName })
            .ToList();

        var model = new EditBookingViewModel
        {
            BookingId        = booking.BookingId,
            CourtId          = courtId ?? "",
            BookingDate      = bookDateVal,
            CurrentHours     = currentHours,
            TakenHours       = takenHours,
            SelectedHoursRaw = string.Join(",", currentHours),
            Courts           = courts
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult EditBooking(EditBookingViewModel data)
    {
        var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == data.BookingId);
        if (booking == null) return RedirectToAction("Bookings");

        booking.CourtId  = data.CourtId;
        booking.BookDate = data.BookingDate;
        booking.Price    = data.SelectedHoursRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries).Length * 200m;
        _db.Update(booking);

        var oldSlots = _db.BookTimes.Where(bt => bt.BookingId == data.BookingId).ToList();
        _db.RemoveRange(oldSlots);

        foreach (var h in data.SelectedHoursRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse))
        {
            _db.Add(new BookTime
            {
                BtId         = Guid.NewGuid().ToString(),
                BookingId    = data.BookingId,
                CourtId      = data.CourtId,
                Booktimeslot = h.ToString("D2") + ":00",
                BookDate     = data.BookingDate
            });
        }

        _db.SaveChanges();
        return RedirectToAction("Bookings");
    }
}
