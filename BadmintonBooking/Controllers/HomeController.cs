using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BadmintonBooking.Models;
using BadmintonBooking.Models.Db;
using BadmintonBooking.ViewModels;

namespace BadmintonBooking.Controllers;

public class HomeController : Controller
{
    private readonly Csi402dbContext _db;
    public HomeController(Csi402dbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Lab_9()
    {
        // var user = (from u in _db.Usersdata select u).ToList();
        return View();
    }
    [HttpPost]
    public IActionResult Lab_9(Lab9UserViewModel data)
    {
        var u = new Labstudent();
        u.StdId = data.UserId;
        u.StdName = data.Name;
        u.StdLastname = data.Lastname;
        u.StdPassword = data.Password;
        _db.Add(u);
        _db.SaveChanges();
        return RedirectToAction("Lab_9List","Home");
    }

    public IActionResult Lab_9List()
    {
        var user = (from u in _db.Labstudents select new Lab9UserViewModel 
        { 
            UserId = u.StdId, 
            Name = u.StdName, 
            Lastname = u.StdLastname, 
            Password = u.StdPassword 
        }).ToList();
        return View(user);
    }

    public IActionResult Lab_10(string UID)
    {
        var check = (from u in _db.Labstudents where u.StdId == UID select new Lab9UserViewModel 
        { 
            UserId = u.StdId, 
            Name = u.StdName, 
            Lastname = u.StdLastname, 
            Password = u.StdPassword 
        }).FirstOrDefault();
        return View(check);
    }

    [HttpPost]
    public IActionResult Lab_10(Lab9UserViewModel data)
    {
        var user = (from u in _db.Labstudents where u.StdId == data.UserId select u).FirstOrDefault();

        user.StdName = data.Name;
        user.StdLastname = data.Lastname;
        user.StdPassword = data.Password;

        _db.Update(user);
        _db.SaveChanges();
        return RedirectToAction("Lab_9List", "Home");
    }

    public IActionResult Lab_10D(string UID)
    {
        var user = (from u in _db.Labstudents where u.StdId == UID select u).FirstOrDefault();
        
        _db.RemoveRange(user);
        _db.SaveChanges();
        return RedirectToAction("Lab_9List", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SPU()
    {
        // ประกาศตัวแปร
        {
            string name = "กันณพิช เบ้าศรี";
            string section = "Sec 004";
            string year = "ปี 3";
            string language = "HTML, CSS, JS";

            // จัดการคะแนน
            int[] scores = { 4, 4, 7, 3, 9, 6, 5, 3, 2, 2 }; // คะแนน 10 ครั้ง
            int totalScore = 0;
            List<string> lowScoresInfo = new List<string>();

            for (int i = 0; i < scores.Length; i++)
            {
                totalScore += scores[i];

                if (scores[i] < 5)
                {
                    // i + i เพราะคอมพิวเตอร์เริ่มนับจาก 0 แต่ทั่วไปนับ 1
                    lowScoresInfo.Add($"งานที่ {i + 1}: {scores[i]} คะแนน");
                }
            }

            // ตัดเกรด
            string grade = CalculateGrade(totalScore);

            // ส่งค่าไปที่ View
            ViewBag.Name = name;
            ViewBag.Section = section;
            ViewBag.Year = year;
            ViewBag.Language = language;
            ViewBag.TotalScore = totalScore;
            ViewBag.Grade = grade;
            ViewBag.LowScoresInfo = lowScoresInfo;

            return View();

        }

    }

    private string CalculateGrade(int totalScore)
    {
        if (totalScore >= 80) return "A";
        if (totalScore >= 76) return "B+";
        if (totalScore >= 70) return "B";
        if (totalScore >= 66) return "C+";
        if (totalScore >= 60) return "C";
        if (totalScore >= 56) return "D+";
        if (totalScore >= 50) return "D";
        return "F";
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
