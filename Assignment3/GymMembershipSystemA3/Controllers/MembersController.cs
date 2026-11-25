using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchName,
            string? searchPhone,
            string? searchEmail)
        {
            IQueryable<GymMember> members = _context.GymMembers
                .AsNoTracking()
                .Where(m => m.Role == "Member");

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                searchName = searchName.Trim();

                string name = searchName.ToLowerInvariant();

                members = members.Where(m =>
                    m.FullName.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(searchPhone))
            {
                searchPhone = searchPhone.Trim();

                members = members.Where(m =>
                    m.PhoneNumber.Contains(searchPhone));
            }

            if (!string.IsNullOrWhiteSpace(searchEmail))
            {
                searchEmail = searchEmail.Trim();

                string email = searchEmail.ToLowerInvariant();

                members = members.Where(m =>
                    m.Email.ToLower().Contains(email));
            }

            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;

            var results = await members
                .OrderBy(m => m.FullName)
                .ToListAsync();

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.GymMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.Role == "Member");

            if (member == null)
            {
                return NotFound();
            }

            var subscriptions = await _context.GymSubscriptions
                .AsNoTracking()
                .Include(s => s.MembershipPlan)
                .Where(s => s.GymMemberId == id)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var visits = await _context.GymVisits
                .AsNoTracking()
                .Where(v => v.GymMemberId == id)
                .OrderByDescending(v => v.Id)
                .Take(30)
                .ToListAsync();

            var model = new MemberDetailsViewModel
            {
                Member = member,
                Subscriptions = subscriptions,
                Visits = visits
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.GymMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.Role == "Member");

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string? fullName,
            string? phoneNumber)
        {
            var member = await _context.GymMembers
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.Role == "Member");

            if (member == null)
            {
                return NotFound();
            }

            fullName = fullName?.Trim() ?? "";
            phoneNumber = phoneNumber?.Trim() ?? "";

            ViewBag.EnteredFullName = fullName;
            ViewBag.EnteredPhone = phoneNumber;

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                ViewBag.Message =
                    "Full name must contain between 2 and 100 characters.";

                return View(member);
            }

            if (phoneNumber.Length != 10 ||
                !phoneNumber.StartsWith("05") ||
                !phoneNumber.All(c => c >= '0' && c <= '9'))
            {
                ViewBag.Message =
                    "Phone number must start with 05 and contain exactly 10 digits.";

                return View(member);
            }

            bool phoneExists = await _context.GymMembers
                .AnyAsync(m =>
                    m.Id != id &&
                    m.PhoneNumber == phoneNumber);

            if (phoneExists)
            {
                ViewBag.Message =
                    "This phone number is already registered.";

                return View(member);
            }

            member.FullName = fullName;
            member.PhoneNumber = phoneNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewBag.Message =
                    "Unable to update member details. Please try again.";

                return View(member);
            }

            TempData["Message"] =
                "Member details updated successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}