using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembershipPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchString)
        {
            var plans = _context.MembershipPlans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                plans = plans.Where(p => p.Name.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View(await plans.OrderBy(p => p.Id).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,DurationMonths,Price")] MembershipPlan plan)
        {
            if (string.IsNullOrWhiteSpace(plan.Name) ||
                plan.DurationMonths <= 0 ||
                plan.Price <= 0)
            {
                ModelState.AddModelError("", "Please enter valid plan details.");
            }

            if (!ModelState.IsValid)
            {
                return View(plan);
            }

            _context.MembershipPlans.Add(plan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.MembershipPlans.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,DurationMonths,Price")] MembershipPlan plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(plan.Name) ||
                plan.DurationMonths <= 0 ||
                plan.Price <= 0)
            {
                ModelState.AddModelError("", "Please enter valid plan details.");
            }

            if (!ModelState.IsValid)
            {
                return View(plan);
            }

            var existingPlan = await _context.MembershipPlans.FindAsync(id);

            if (existingPlan == null)
            {
                return NotFound();
            }

            existingPlan.Name = plan.Name;
            existingPlan.DurationMonths = plan.DurationMonths;
            existingPlan.Price = plan.Price;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.MembershipPlans.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.MembershipPlans.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            _context.MembershipPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}