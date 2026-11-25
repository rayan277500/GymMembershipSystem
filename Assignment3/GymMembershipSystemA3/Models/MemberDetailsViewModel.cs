namespace GymMembershipSystemA3.Models
{
    public class MemberDetailsViewModel
    {
        public GymMember Member { get; set; } = null!;

        public List<GymSubscription> Subscriptions { get; set; } = new();

        public List<GymVisit> Visits { get; set; } = new();
    }
}