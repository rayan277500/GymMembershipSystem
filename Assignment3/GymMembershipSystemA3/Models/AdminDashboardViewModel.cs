namespace GymMembershipSystemA3.Models
{
    public class AdminDashboardViewModel
    {
        public int MemberCount { get; set; }

        public int AdminCount { get; set; }

        public int PlanCount { get; set; }

        public List<AdminMemberViewModel> Members { get; set; } = new();
    }

    public class AdminMemberViewModel
    {
        public string FullName { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Email { get; set; } = "";

        public string Role { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}