namespace GymMembershipSystemA3.Models
{
    public class MembershipPlan
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public int DurationMonths { get; set; }

        public decimal Price { get; set; }
    }
}