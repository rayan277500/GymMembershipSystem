using System.Security.Claims;
using GymMembershipSystemA3.Data;
using GymMembershipSystemA3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMembershipSystemA3.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<GymMember> _passwordHasher;

        public AccountController(
            ApplicationDbContext context,
            IPasswordHasher<GymMember> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email, string? password)
        {
            string cleanEmail = email?.Trim().ToLowerInvariant() ?? "";
            string cleanPassword = password ?? "";

            if (string.IsNullOrWhiteSpace(cleanEmail) ||
                string.IsNullOrWhiteSpace(cleanPassword))
            {
                ViewBag.Message = "Please fill in all fields.";
                return View();
            }

            if (cleanPassword.Length < 8)
            {
                ViewBag.Message = "Password must be at least 8 characters.";
                return View();
            }

            var member = await _context.GymMembers
                .FirstOrDefaultAsync(m => m.Email == cleanEmail);

            if (member == null)
            {
                ViewBag.Message = "Invalid email or password.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(
                member,
                member.PasswordHash,
                cleanPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Message = "Invalid email or password.";
                return View();
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                member.PasswordHash = _passwordHasher.HashPassword(
                    member,
                    cleanPassword);

                await _context.SaveChangesAsync();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new Claim(ClaimTypes.Name, member.FullName),
                new Claim(ClaimTypes.Email, member.Email),
                new Claim(ClaimTypes.Role, member.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false
                });

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Signup()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(
            string? fullname,
            string? phone,
            string? email,
            string? password)
        {
            string cleanName = fullname?.Trim() ?? "";
            string cleanPhone = phone?.Trim() ?? "";
            string cleanEmail = email?.Trim().ToLowerInvariant() ?? "";
            string cleanPassword = password ?? "";

            if (string.IsNullOrWhiteSpace(cleanName) ||
                string.IsNullOrWhiteSpace(cleanPhone) ||
                string.IsNullOrWhiteSpace(cleanEmail) ||
                string.IsNullOrWhiteSpace(cleanPassword))
            {
                ViewBag.Message = "Please fill in all fields.";
                return View();
            }

            if (!cleanPhone.StartsWith("05"))
            {
                ViewBag.Message = "Please enter a Saudi phone number starting with 05.";
                return View();
            }

            if (cleanPhone.Length != 10)
            {
                ViewBag.Message = "Phone number must contain exactly 10 digits.";
                return View();
            }

            foreach (char digit in cleanPhone)
            {
                if (digit < '0' || digit > '9')
                {
                    ViewBag.Message = "Phone number must contain numbers only.";
                    return View();
                }
            }

            if (!System.Net.Mail.MailAddress.TryCreate(
                    cleanEmail,
                    out var emailAddress) ||
                emailAddress.Address != cleanEmail ||
                cleanEmail.LastIndexOf('.') <= cleanEmail.IndexOf('@') + 1 ||
                cleanEmail.EndsWith('.'))
            {
                ViewBag.Message = "Please enter a valid email address.";
                return View();
            }

            if (cleanPassword.Length < 8)
            {
                ViewBag.Message = "Password must be at least 8 characters.";
                return View();
            }

            if (cleanPassword.Length > 128)
            {
                ViewBag.Message = "Password is too long.";
                return View();
            }

            bool emailExists = await _context.GymMembers
                .AnyAsync(m => m.Email == cleanEmail);

            if (emailExists)
            {
                ViewBag.Message = "This email address is already registered.";
                return View();
            }

            bool phoneExists = await _context.GymMembers
                .AnyAsync(m => m.PhoneNumber == cleanPhone);

            if (phoneExists)
            {
                ViewBag.Message = "This phone number is already registered.";
                return View();
            }

            var member = new GymMember
            {
                FullName = cleanName,
                PhoneNumber = cleanPhone,
                Email = cleanEmail,
                Role = "Member",
                CreatedAt = DateTime.UtcNow
            };

            member.PasswordHash = _passwordHasher.HashPassword(
                member,
                cleanPassword);

            try
            {
                _context.GymMembers.Add(member);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewBag.Message =
                    "Unable to create account. Please check your details.";

                return View();
            }

            TempData["SuccessMessage"] =
                "Account created successfully. Please log in.";

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return StatusCode(403, "Access denied.");
        }
    }
}