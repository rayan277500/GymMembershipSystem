using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers
{
    [Authorize(Roles = "Member,Admin")]
    public class GymOccupancyController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const int Capacity = 50;

        private static readonly TimeZoneInfo SaudiTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh");

        public GymOccupancyController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static DateTime GetSaudiTime()
        {
            return TimeZoneInfo.ConvertTime(
                DateTimeOffset.Now,
                SaudiTimeZone).DateTime;
        }

        private int GetCurrentMemberId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out int id) ? id : 0;
        }

        private async Task<bool> HasActiveMembership(int memberId)
        {
            DateTime now = DateTime.UtcNow;

            return await _context.GymSubscriptions.AnyAsync(s =>
                s.GymMemberId == memberId &&
                (s.Status == "Active" || s.Status == "Renewed") &&
                s.StartDate.HasValue &&
                s.StartDate.Value <= now &&
                s.EndDate.HasValue &&
                s.EndDate.Value > now);
        }

        private static string GetCrowdLevel(int insideCount)
        {
            if (insideCount >= Capacity)
            {
                return "Full";
            }

            double percentage = insideCount * 100.0 / Capacity;

            if (percentage >= 75)
            {
                return "Busy";
            }

            if (percentage >= 40)
            {
                return "Moderate";
            }

            return "Low";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int insideCount = await _context.GymVisits
                .CountAsync(v => v.CheckOutAt == null);

            var model = new GymOccupancyViewModel
            {
                Capacity = Capacity,
                InsideCount = insideCount
            };

            if (User.IsInRole("Member"))
            {
                int memberId = GetCurrentMemberId();

                if (memberId == 0)
                {
                    return Forbid();
                }

                model.HasActiveSubscription =
                    await HasActiveMembership(memberId);

                model.IsCheckedIn = await _context.GymVisits
                    .AnyAsync(v =>
                        v.GymMemberId == memberId &&
                        v.CheckOutAt == null);

                ViewBag.VisitHistory = await _context.GymVisits
                    .AsNoTracking()
                    .Where(v => v.GymMemberId == memberId)
                    .OrderByDescending(v => v.Id)
                    .Take(30)
                    .ToListAsync();
            }

            if (User.IsInRole("Admin"))
            {
                model.VisitorsInside = await _context.GymVisits
                    .AsNoTracking()
                    .Include(v => v.GymMember)
                    .Where(v => v.CheckOutAt == null)
                    .OrderBy(v => v.CheckInAt)
                    .ToListAsync();

                ViewBag.VisitHistory = await _context.GymVisits
                    .AsNoTracking()
                    .Include(v => v.GymMember)
                    .OrderByDescending(v => v.Id)
                    .Take(50)
                    .ToListAsync();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Status()
        {
            int insideCount = await _context.GymVisits
                .CountAsync(v => v.CheckOutAt == null);

            int percentage = (int)Math.Round(
                insideCount * 100.0 / Capacity);

            return Json(new
            {
                insideCount,
                capacity = Capacity,
                availablePlaces = Math.Max(0, Capacity - insideCount),
                percentage,
                crowdLevel = GetCrowdLevel(insideCount)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CheckIn()
        {
            int memberId = GetCurrentMemberId();

            if (memberId == 0)
            {
                return Forbid();
            }

            if (!await HasActiveMembership(memberId))
            {
                TempData["Message"] =
                    "You need an active membership to check in.";

                return RedirectToAction(nameof(Index));
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            bool alreadyInside = await _context.GymVisits
                .AnyAsync(v =>
                    v.GymMemberId == memberId &&
                    v.CheckOutAt == null);

            if (alreadyInside)
            {
                TempData["Message"] = "You are already checked in.";

                return RedirectToAction(nameof(Index));
            }

            int insideCount = await _context.GymVisits
                .CountAsync(v => v.CheckOutAt == null);

            if (insideCount >= Capacity)
            {
                TempData["Message"] = "The gym is currently full.";

                return RedirectToAction(nameof(Index));
            }

            var visit = new GymVisit
            {
                GymMemberId = memberId,
                CheckInAt = GetSaudiTime()
            };

            _context.GymVisits.Add(visit);

            try
            {
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Message"] = "Check-in successful.";
            }
            catch (DbUpdateException)
            {
                TempData["Message"] =
                    "Check-in could not be completed. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CheckOut()
        {
            int memberId = GetCurrentMemberId();

            if (memberId == 0)
            {
                return Forbid();
            }

            var visit = await _context.GymVisits
                .FirstOrDefaultAsync(v =>
                    v.GymMemberId == memberId &&
                    v.CheckOutAt == null);

            if (visit == null)
            {
                TempData["Message"] =
                    "You are not currently checked in.";

                return RedirectToAction(nameof(Index));
            }

            visit.CheckOutAt = GetSaudiTime();

            await _context.SaveChangesAsync();

            TempData["Message"] = "Check-out successful.";

            return RedirectToAction(nameof(Index));
        }
    }
}