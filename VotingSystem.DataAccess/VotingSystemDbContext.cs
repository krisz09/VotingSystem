using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess
{

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

        public async Task SeedAsync(UserManager<User> userManager)
        {
            var existingUser = await userManager.FindByIdAsync("1");
            if (existingUser == null)
            {
                var user = new User
                {
                    Id = "1",
                    UserName = "admin",
                    Email = "admin@example.com"
                };

                var result = await userManager.CreateAsync(user, "Admin123!"); // adsz jelszót is

                if (!result.Succeeded)
                {
                    Console.WriteLine("User creation failed:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }

                    return;
                }
            }

            if (!Polls.Any())
            {
                var polls = new List<Poll>
        {
            new Poll
            {
                Question = "What is your favorite color?",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
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
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
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

                var pollCount = Polls.Count();
                Console.WriteLine($"Poll count after save: {pollCount}");

                foreach (var poll in Polls.Include(p => p.PollOptions))
                {
                    Console.WriteLine($"Poll: {poll.Question}, Start: {poll.StartDate}, End: {poll.EndDate}, Options: {poll.PollOptions.Count}");
                }

            }
        }
    }
}
