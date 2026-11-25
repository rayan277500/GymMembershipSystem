using Microsoft.EntityFrameworkCore;
using GymMembershipSystemA3.Models;

namespace GymMembershipSystemA3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MembershipPlan> MembershipPlans
        {
            get { return Set<MembershipPlan>(); }
        }

        public DbSet<GymMember> GymMembers
        {
            get { return Set<GymMember>(); }
        }

        public DbSet<GymSubscription> GymSubscriptions
        {
            get { return Set<GymSubscription>(); }
        }

        public DbSet<GymVisit> GymVisits
        {
            get { return Set<GymVisit>(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GymMember>()
                .HasIndex(member => member.Email)
                .IsUnique();

            modelBuilder.Entity<GymMember>()
                .HasIndex(member => member.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<GymSubscription>()
                .HasOne(subscription => subscription.GymMember)
                .WithMany()
                .HasForeignKey(subscription => subscription.GymMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymSubscription>()
                .HasOne(subscription => subscription.MembershipPlan)
                .WithMany()
                .HasForeignKey(subscription => subscription.MembershipPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymVisit>()
                .HasOne(visit => visit.GymMember)
                .WithMany()
                .HasForeignKey(visit => visit.GymMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymVisit>()
                .HasIndex(visit => visit.GymMemberId)
                .IsUnique()
                .HasFilter("\"CheckOutAt\" IS NULL");
        }
    }
}