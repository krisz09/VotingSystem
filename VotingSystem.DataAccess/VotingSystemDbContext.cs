using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess {

    public class VotingSystemDbContext : IdentityDbContext<User>
    {
        public VotingSystemDbContext(DbContextOptions<VotingSystemDbContext> options) : base(options) { }

        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // Egy felhasználó egy adott szavazáson csak egyszer szavazhat
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void Seed()
        {
            if (!Users.Any(u => u.Id == "1"))
            {
                Users.Add(new User { Id = "1", UserName = "admin" });
                SaveChanges();
            }

            if (!Polls.Any())
            {
                var polls = new List<Poll>
                {
                    new Poll
                    {
                        Question = "What is your favorite color?",
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(10),
                        CreatedByUserId = "1",
                        PollOptions = new List<PollOption>
                        {
                            new PollOption { OptionText = "Red" },
                            new PollOption { OptionText = "Blue" },
                            new PollOption { OptionText = "Green" }
                        }
                    },
                    new Poll
                    {
                        Question = "What is your favorite programming language?",
                        StartDate = DateTime.Now.AddDays(-1),
                        EndDate = DateTime.Now.AddDays(10),
                        CreatedByUserId = "1",
                        PollOptions = new List<PollOption>
                        {
                            new PollOption { OptionText = "C#" },
                            new PollOption { OptionText = "Java" },
                            new PollOption { OptionText = "Python" }
                        }
                    }
                };

                Polls.AddRange(polls);
                SaveChanges();
            }
        }
    }
}