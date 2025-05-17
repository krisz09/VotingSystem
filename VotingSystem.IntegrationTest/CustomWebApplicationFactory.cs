using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Models;

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

                // Build the service provider
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VotingSystemDbContext>();
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                db.Database.EnsureCreated();

                // Seed roles
                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                }

                // Seed test data
                db.Polls.Add(new Poll
                {
                    Question = "Test Poll",
                    CreatedByUserId = "abc",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    Minvotes = 1,
                    Maxvotes = 2,
                    PollOptions = new System.Collections.Generic.List<PollOption>
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