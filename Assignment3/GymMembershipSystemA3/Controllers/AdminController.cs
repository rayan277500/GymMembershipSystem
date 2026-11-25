using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                MemberCount = await _context.GymMembers
                    .CountAsync(m => m.Role == "Member"),

                AdminCount = await _context.GymMembers
                    .CountAsync(m => m.Role == "Admin"),

                PlanCount = await _context.MembershipPlans
                    .CountAsync(),

                Members = await _context.GymMembers
                    .AsNoTracking()
                    .OrderBy(m => m.Id)
                    .Select(m => new AdminMemberViewModel
                    {
                        FullName = m.FullName,
                        PhoneNumber = m.PhoneNumber,
                        Email = m.Email,
                        Role = m.Role,
                        CreatedAt = m.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}