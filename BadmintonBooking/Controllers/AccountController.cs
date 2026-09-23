using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BadmintonBooking.Models;
using BadmintonBooking.ViewModels;
using BadmintonBooking.Models.Db;

namespace BadmintonBooking.Controllers;

public class AccountController : Controller
{
    private readonly Csi402dbContext _db;

    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger, Csi402dbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Test1()
    {
        return View();
    }

    public IActionResult User_home(string Username, string Email, string Phone)
    {
        ViewBag.Username = Username;
        ViewBag.Email = Email;
        ViewBag.Phone = Phone;
        return View();
    }
    public IActionResult Lab_4()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Login(LoginViewModel data)
    {
        var user = _db.Usersdata
            .FirstOrDefault(u => u.Username == data.Username && u.Password == data.Password);

        if (user == null)
            return View();

        HttpContext.Session.SetString("UserId", user.UserId);
        HttpContext.Session.SetString("Username", user.Username ?? "");
        var role = _db.Roles.FirstOrDefault(r => r.RoleId == user.Role);
        HttpContext.Session.SetString("Role", role?.RoleName ?? "");
        HttpContext.Session.SetInt32("AccessLevel", role?.AccessLevel ?? 1);
        HttpContext.Session.SetInt32("IsMember", user.IsMember ? 1 : 0);

        return RedirectToAction("User_home","Account", new { Username = user.Username});
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(RegisterViewModel data)
    {
        var user = new Usersdatum();
        user.UserId = Guid.NewGuid().ToString();  // auto generate unique string ID
        user.Username = data.Username;
        user.Email = data.Email;
        user.Phone = data.Phone;
        user.Password = data.Password;
        user.Role = "1";
        _db.Add(user);
        _db.SaveChanges();
        return RedirectToAction("User_home","Account");
    }
    public IActionResult User_manager()
    {
        return View();
    }

    public IActionResult Lab_5()
    {
        // var User = new LabUserViewModel();
        // User.UserId = 1;
        // User.Username = "Kannapich";
        // User.Email = "kannapich@example.com";
        // User.Phone = "0812345678";
        // User.Role = "Admin";

        // var User = new List<LabUserViewModel>
        // {
        //     new LabUserViewModel
        //     {
        //         UserId = 1,
        //         Username = "Kannapich",
        //         Email = "kannapich@example.com",
        //         Phone = "0812345678",
        //         Weight = 70.5m,
        //         Height = 1.75m,
        //         Age = 22,
        //         Role = "Admin"
        //     },
        //     new LabUserViewModel
        //     {
        //         UserId = 2,
        //         Username = "John",
        //         Email = "john@example.com",
        //         Phone = "0898765432",
        //         Weight = 80.0m,
        //         Height = 1.80m,
        //         Age = 25,
        //         Role = "User"
        //     },
        //     new LabUserViewModel
        //     {
        //         UserId = 3,
        //         Username = "Jane",
        //         Email = "jane@example.com",
        //         Phone = "0876543210",
        //         Weight = 60.0m,
        //         Height = 1.65m,
        //         Age = 28,
        //         Role = "Admin"
        //     }
        // };
        return View();
    }
    [HttpPost]
    public IActionResult Lab_5(LabUserViewModel data)
    {
        int a;
        string b,c;
        a = data.UserId;
        b = data.Username;
        c = data.Email;
        ViewBag.UserId = a;
        ViewBag.Username = b;
        ViewBag.Email = c;
        return RedirectToAction("Lab_52","Account",new { UserId = a, Username = b, Email = c });
    }

    public IActionResult Lab_52(int UserId, string Username, string Email)
    {
        ViewBag.UserId = UserId;
        ViewBag.Username = Username;
        ViewBag.Email = Email;
        return View();
    }

    public IActionResult Lab8()
    {
        var user = (from u in _db.Usersdata select u).ToList();
        return View(user);
    }

    public IActionResult EditUser(string UID)
    {
        var check = (from u in _db.Usersdata where u.UserId == UID select new UserDataViewModel
        {
            UserId = u.UserId,
            Username = u.Username,
            Phone = u.Phone,
            Email = u.Email,
            Role = u.Role,
            Password = u.Password
        }).FirstOrDefault();
        return View(check);
    }

    [HttpPost]
    public IActionResult EditUser(UserDataViewModel data)
    {
        var user = (from u in _db.Usersdata where u.UserId == data.UserId select u).FirstOrDefault();
        user.Username = data.Username;
        user.Phone = data.Phone;
        user.Email = data.Email;
        user.Role = data.Role;
        user.Password = data.Password;
        _db.Update(user);
        _db.SaveChanges();
        return RedirectToAction("Lab8", "Account");
    }

    public IActionResult DeleteUser(string UID)
    {
        var user = (from u in _db.Usersdata where u.UserId == UID select u).FirstOrDefault();
        _db.RemoveRange(user);
        _db.SaveChanges();
        return RedirectToAction("Lab8", "Account");
    }

    public IActionResult Courts()
    {
        var courts = (from c in _db.Courts
                    select new CourtsViewModel
                    {
                        CourtId = c.CourtId,
                        CourtName = c.CourtName
                    }).ToList();
        return View(courts);
    }

    public IActionResult Bookingform(string CID, string UID, string? date = null)
    {
        var bookingDate = date != null
            ? DateOnly.Parse(date)
            : DateOnly.FromDateTime(DateTime.Today);

        var takenHours = _db.BookTimes
            .Where(bt => bt.CourtId == CID && bt.BookDate == bookingDate)
            .Select(bt => bt.Booktimeslot)
            .ToList()
            .Where(s => s != null)
            .Select(s => int.Parse(s!.Split(':')[0]))
            .Distinct()
            .ToList();

        var booking = new BookingViewModel
        {
            UserId = UID,
            CourtId = CID,
            BookingDate = bookingDate,
            TakenHours = takenHours
        };

        return View(booking);
    }

    [HttpPost]
    public IActionResult Bookingform(BookingViewModel data)
    {
        var hours = data.SelectedHoursRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToList();
        data.Price = hours.Count * 200m;

        TempData["B_UserId"]        = data.UserId;
        TempData["B_CourtId"]       = data.CourtId;
        TempData["B_BookingDate"]   = data.BookingDate.ToString("yyyy-MM-dd");
        TempData["B_SelectedHours"] = data.SelectedHoursRaw;
        TempData["B_Price"]         = data.Price.ToString();
        return RedirectToAction("payment", "Account");
    }

    public IActionResult payment()
    {
        if (TempData["B_CourtId"] is not string courtId)
            return RedirectToAction("Courts", "Account");

        var userId           = TempData["B_UserId"]?.ToString() ?? "";
        var selectedHoursRaw = TempData["B_SelectedHours"]!.ToString()!;
        var basePrice        = decimal.Parse(TempData["B_Price"]!.ToString()!);

        // TempData JSON serializer auto-converts ISO date strings to DateTime objects
        var rawDate     = TempData["B_BookingDate"]!;
        var bookingDate = rawDate is DateTime dt
            ? DateOnly.FromDateTime(dt)
            : DateOnly.Parse(rawDate.ToString()!);
        var dateStr     = bookingDate.ToString("yyyy-MM-dd");
        var selectedHours = selectedHoursRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToList();
        var hours = selectedHours.Count;

        var isMember      = HttpContext.Session.GetInt32("IsMember") == 1;
        var bookingCount  = _db.Bookings.Count(b => b.UserId == userId);
        var daysUntilBook = (bookingDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).TotalDays;
        var isWeekday     = bookingDate.DayOfWeek != DayOfWeek.Saturday && bookingDate.DayOfWeek != DayOfWeek.Sunday;

        var applied = new List<AppliedPromotion>();
        foreach (var promo in _db.Promotions.ToList())
        {
            bool applies = promo.ConditionType switch
            {
                "HOUR"   => hours >= promo.ConditionValue,
                "MEMBER" => isMember,
                "COUNT"  => bookingCount >= promo.ConditionValue,
                "DAY"    => daysUntilBook >= promo.ConditionValue,
                "DATE"   => isWeekday,
                _        => false
            };
            if (applies)
            {
                var displayName = promo.ConditionType switch
                {
                    "HOUR"   => $"Book {promo.ConditionValue}+ Hours Discount",
                    "MEMBER" => "Member Discount",
                    "COUNT"  => $"Loyal Customer Discount (#{promo.ConditionValue}+ Bookings)",
                    "DAY"    => $"{promo.ConditionValue}+ Days Ahead",
                    "DATE"   => "Weekday Discount",
                    _        => promo.PromotionName ?? promo.PromotionId
                };
                applied.Add(new AppliedPromotion { Name = displayName, Discount = promo.DiscountValue ?? 0 });
            }
        }

        var totalDiscount = applied.Sum(p => p.Discount);

        var model = new PaymentViewModel
        {
            UserId            = userId,
            CourtId           = courtId,
            BookingDate       = dateStr,
            SelectedHoursRaw  = selectedHoursRaw,
            BasePrice         = basePrice,
            Price             = Math.Max(0, basePrice - totalDiscount),
            AppliedPromotions = applied
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult payment(string UserId, string CourtId, string BookingDate, string SelectedHoursRaw, decimal Price)
    {
        var bookingId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        var bookDate  = DateOnly.FromDateTime(DateTime.Parse(BookingDate));

        var booking = new Booking
        {
            BookingId = bookingId,
            UserId    = UserId,
            CourtId   = CourtId,
            BookDate  = bookDate,
            Price     = Price
        };
        _db.Add(booking);

        foreach (var h in SelectedHoursRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
        {
            _db.Add(new BookTime
            {
                BtId         = Guid.NewGuid().ToString(),
                BookingId    = bookingId,
                CourtId      = CourtId,
                Booktimeslot = h.ToString("D2") + ":00",
                BookDate     = bookDate
            });
        }

        _db.SaveChanges();
        return RedirectToAction("Booking", "Account");
    }

    public IActionResult Booking()
    {
        var userId     = HttpContext.Session.GetString("UserId");
        var bookings   = _db.Bookings.Where(b => b.UserId == userId).ToList();
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

    public IActionResult setting()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var user = _db.Usersdata.FirstOrDefault(u => u.UserId == userId);
        if (user == null) return RedirectToAction("Login", "Account");

        var model = new SettingViewModel
        {
            UserId   = user.UserId,
            Username = user.Username ?? "",
            Email    = user.Email ?? "",
            Phone    = user.Phone ?? "",
            Password = user.Password ?? "",
            IsMember = user.IsMember
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult setting(SettingViewModel data)
    {
        var user = _db.Usersdata.FirstOrDefault(u => u.UserId == data.UserId);
        if (user == null) return RedirectToAction("Login", "Account");

        user.Username = data.Username;
        user.Email    = data.Email;
        user.Phone    = data.Phone;
        user.Password = data.Password;
        user.IsMember = data.IsMember;
        _db.Update(user);
        _db.SaveChanges();

        HttpContext.Session.SetString("Username", data.Username);
        HttpContext.Session.SetInt32("IsMember", data.IsMember ? 1 : 0);

        return RedirectToAction("setting", "Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
