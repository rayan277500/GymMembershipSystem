namespace GymMembershipSystemA3.Models
{
    public class GymVisit
    {
        public int Id { get; set; }

        public int GymMemberId { get; set; }

        public GymMember GymMember { get; set; } = null!;

        public DateTime CheckInAt { get; set; } = DateTime.UtcNow;

        public DateTime? CheckOutAt { get; set; }
    }
}