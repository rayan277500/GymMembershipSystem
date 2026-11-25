using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers
{
    [Authorize]
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentMemberId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out int id) ? id : 0;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> My()
        {
            int memberId = GetCurrentMemberId();

            if (memberId == 0)
            {
                return Forbid();
            }

            var subscriptions = await _context.GymSubscriptions
                .AsNoTracking()
                .Include(s => s.MembershipPlan)
                .Where(s => s.GymMemberId == memberId)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return View(subscriptions);
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Subscribe(int planId)
        {
            int memberId = GetCurrentMemberId();

            if (memberId == 0)
            {
                return Forbid();
            }

            var plan = await _context.MembershipPlans.FindAsync(planId);

            if (plan == null)
            {
                return NotFound();
            }

            var subscriptions = await _context.GymSubscriptions
                .AsNoTracking()
                .Where(s => s.GymMemberId == memberId)
                .ToListAsync();

            if (subscriptions.Any(s => s.Status == "Pending"))
            {
                TempData["Message"] = "You already have a pending subscription request.";

                return RedirectToAction(nameof(My));
            }

            var latestApproved = subscriptions
                .Where(s => (s.Status == "Active" || s.Status == "Renewed") &&
                    s.EndDate.HasValue &&
                    s.EndDate.Value > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            if (latestApproved != null &&
                latestApproved.MembershipPlanId != planId)
            {
                TempData["Message"] = "You can only renew your current plan while it is active.";

                return RedirectToAction(nameof(My));
            }

            ViewBag.IsRenewal = latestApproved != null;

            if (latestApproved?.EndDate != null)
            {
                ViewBag.CurrentEndDate = latestApproved.EndDate.Value
                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                ViewBag.NewEndDate = latestApproved.EndDate.Value
                    .AddMonths(plan.DurationMonths)
                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            return View(plan);
        }

        [HttpPost, ActionName("Subscribe")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> SubscribeConfirmed(int planId)
        {
            int memberId = GetCurrentMemberId();

            if (memberId == 0)
            {
                return Forbid();
            }

            var plan = await _context.MembershipPlans.FindAsync(planId);

            if (plan == null)
            {
                return NotFound();
            }

            var subscriptions = await _context.GymSubscriptions
                .AsNoTracking()
                .Where(s => s.GymMemberId == memberId)
                .ToListAsync();

            if (subscriptions.Any(s => s.Status == "Pending"))
            {
                TempData["Message"] = "You already have a pending subscription request.";

                return RedirectToAction(nameof(My));
            }

            var latestApproved = subscriptions
                .Where(s => (s.Status == "Active" || s.Status == "Renewed") &&
                    s.EndDate.HasValue &&
                    s.EndDate.Value > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            if (latestApproved != null &&
                latestApproved.MembershipPlanId != planId)
            {
                TempData["Message"] = "You can only renew your current plan while it is active.";

                return RedirectToAction(nameof(My));
            }

            var subscription = new GymSubscription
            {
                GymMemberId = memberId,
                MembershipPlanId = planId,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.GymSubscriptions.Add(subscription);

            await _context.SaveChangesAsync();

            TempData["Message"] = latestApproved != null
                ? "Your renewal request has been submitted."
                : "Your subscription request has been submitted.";

            return RedirectToAction(nameof(My));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? searchName,
            string? searchPhone)
        {
            IQueryable<GymSubscription> subscriptions =
                _context.GymSubscriptions
                    .AsNoTracking()
                    .Include(s => s.GymMember)
                    .Include(s => s.MembershipPlan);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                searchName = searchName.Trim();

                string normalizedName = searchName.ToLowerInvariant();

                subscriptions = subscriptions.Where(s =>
                    s.GymMember.FullName.ToLower().Contains(normalizedName));
            }

            if (!string.IsNullOrWhiteSpace(searchPhone))
            {
                searchPhone = searchPhone.Trim();

                subscriptions = subscriptions.Where(s =>
                    s.GymMember.PhoneNumber.Contains(searchPhone));
            }

            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;

            var results = await subscriptions
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return View(results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var subscription = await _context.GymSubscriptions
                .Include(s => s.MembershipPlan)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                return NotFound();
            }

            if (subscription.Status != "Pending")
            {
                return RedirectToAction(nameof(Index));
            }

            DateTime now = DateTime.UtcNow;

            var approvedSubscriptions = await _context.GymSubscriptions
                .AsNoTracking()
                .Where(s =>
                    s.GymMemberId == subscription.GymMemberId &&
                    (s.Status == "Active" || s.Status == "Renewed"))
                .ToListAsync();

            var latestApproved = approvedSubscriptions
                .Where(s =>
                    s.EndDate.HasValue &&
                    s.EndDate.Value > now)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            if (latestApproved != null)
            {
                if (latestApproved.MembershipPlanId !=
                    subscription.MembershipPlanId)
                {
                    TempData["Message"] =
                        "Renewal must use the member's current plan.";

                    return RedirectToAction(nameof(Index));
                }

                DateTime renewalStart =
                    latestApproved.EndDate!.Value;

                subscription.StartDate = renewalStart;

                subscription.EndDate = renewalStart.AddMonths(
                    subscription.MembershipPlan.DurationMonths);

                subscription.Status = "Renewed";

                TempData["Message"] =
                    "Membership renewed successfully.";
            }
            else
            {
                subscription.StartDate = now;

                subscription.EndDate = now.AddMonths(
                    subscription.MembershipPlan.DurationMonths);

                subscription.Status = "Active";

                TempData["Message"] =
                    "Subscription approved successfully.";
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var subscription =
                await _context.GymSubscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            if (subscription.Status != "Pending")
            {
                return RedirectToAction(nameof(Index));
            }

            subscription.Status = "Rejected";

            await _context.SaveChangesAsync();

            TempData["Message"] = "Subscription rejected.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var subscription =
                await _context.GymSubscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            DateTime now = DateTime.UtcNow;

            bool canCancel =
                (subscription.Status == "Active" ||
                 subscription.Status == "Renewed") &&
                subscription.StartDate.HasValue &&
                subscription.EndDate.HasValue &&
                subscription.EndDate.Value > now;

            if (!canCancel)
            {
                TempData["Message"] =
                    "This subscription cannot be cancelled.";

                return RedirectToAction(nameof(Index));
            }

            subscription.Status = "Cancelled";

            await _context.SaveChangesAsync();

            TempData["Message"] =
                "Subscription cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}