namespace GymMembershipSystemA3.Models
{
    public class GymSubscription
    {
        public int Id { get; set; }

        public int GymMemberId { get; set; }

        public GymMember GymMember { get; set; } = null!;

        public int MembershipPlanId { get; set; }

        public MembershipPlan MembershipPlan { get; set; } = null!;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "Pending";
    }
}