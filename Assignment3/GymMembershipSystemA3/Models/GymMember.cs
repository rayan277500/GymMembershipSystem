namespace GymMembershipSystemA3.Models
{
    public class GymMember
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public string Role { get; set; } = "Member";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}