using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("id")))
        {
            HttpContext.Session.SetString("id", Guid.NewGuid().ToString());
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            HttpContext.Session.SetString(
                "username",
                User.Identity.Name ?? "Member"
            );
        }
        else
        {
            HttpContext.Session.SetString("username", "guest");
        }

        var plans = await _context.MembershipPlans
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return View(plans);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult SessionInfo()
    {
        ViewBag.SessionId = HttpContext.Session.GetString("id");

        ViewBag.SessionUsername = HttpContext.Session.GetString("username");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}