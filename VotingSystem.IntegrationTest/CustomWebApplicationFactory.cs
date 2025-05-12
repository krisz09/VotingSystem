using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess;
using VotingSystem.WebApi;

namespace VotingSystem.IntegrationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<WebApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real DB config
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<VotingSystemDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Use in-memory DB
                services.AddDbContext<VotingSystemDbContext>(options =>
                {
                    options.UseInMemoryDatabase("VotingSystemTestDb");
                });

                // Optionally seed data here if needed
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VotingSystemDbContext>();

                db.Database.EnsureCreated();

                // 👉 Seed data
                db.Polls.Add(new Poll
                {
                    Id = 1,
                    Question = "Test Poll",
                    CreatedByUserId = "abc",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    Minvotes = 1,
                    Maxvotes = 2,
                    PollOptions = new List<PollOption>
            {
                new PollOption { OptionText = "Yes" },
                new PollOption { OptionText = "No" }
            }
                });
                db.SaveChanges();
            });
        }
    }
}
