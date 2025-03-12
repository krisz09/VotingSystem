using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess {

    public class VotingSystemDbContext : IdentityDbContext<User>
    {
        public VotingSystemDbContext(DbContextOptions<VotingSystemDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Egy felhasználó egy adott szavazáson csak egyszer szavazhat
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.PollOptionId })
                .IsUnique();

            // Ha egy poll törlődik, törlődjenek az opciói is
            modelBuilder.Entity<Poll>()
                .HasMany(p => p.PollOptions)
                .WithOne(po => po.Poll)
                .HasForeignKey(po => po.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ha egy poll opció törlődik, törlődjenek az arra adott szavazatok is
            modelBuilder.Entity<PollOption>()
                .HasMany(po => po.Votes)
                .WithOne(v => v.PollOption)
                .HasForeignKey(v => v.PollOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}