using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Services;
using Xunit;

namespace VotingSystem.IntegrationTest
{


    public class PollsServiceTests
    {
        private VotingSystemDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<VotingSystemDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new VotingSystemDbContext(options);
        }

        [Fact]
        public async Task CreatePollAsync_ShouldAddPoll()
        {
            // Arrange
            var db = GetDbContext(nameof(CreatePollAsync_ShouldAddPoll));
            var service = new PollsService(db);

            var poll = new Poll
            {
                Question = "Test poll?",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                CreatedByUserId = "user1",
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" },
                    new PollOption { OptionText = "B" }
                }
            };

            // Act
            var result = await service.CreatePollAsync(poll);

            // Assert
            result.Should().BeTrue();
            db.Polls.Count().Should().Be(1);
            db.PollOptions.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAllPollsAsync_ShouldReturnAllPolls()
        {
            // Arrange
            var db = GetDbContext(nameof(GetAllPollsAsync_ShouldReturnAllPolls));
            db.Polls.Add(new Poll { Question = "Q1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Minvotes = 1, Maxvotes = 2 });
            db.Polls.Add(new Poll { Question = "Q2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Minvotes = 1, Maxvotes = 2 });
            db.SaveChanges();
            var service = new PollsService(db);

            // Act
            var polls = await service.GetAllPollsAsync();

            // Assert
            polls.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPollByIdAsync_ShouldReturnPollWithOptions()
        {
            // Arrange
            var db = GetDbContext(nameof(GetPollByIdAsync_ShouldReturnPollWithOptions));
            var poll = new Poll
            {
                Question = "Q",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" }
                }
            };
            db.Polls.Add(poll);
            db.SaveChanges();
            var service = new PollsService(db);

            // Act
            var result = await service.GetPollByIdAsync(poll.Id);

            // Assert
            result.Should().NotBeNull();
            result.PollOptions.Should().HaveCount(1);
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfAlreadyVoted()
        {
            // Arrange
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfAlreadyVoted));
            var poll = new Poll
            {
                Question = "Q",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" }
                }
            };
            db.Polls.Add(poll);
            db.SaveChanges();

            var optionId = db.PollOptions.First().Id;
            db.Votes.Add(new Vote { PollOptionId = optionId, UserId = "user1", VotedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new PollsService(db);

            // Act
            var result = await service.SubmitVotesAsync(new List<int> { optionId }, "user1");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePollAsync_ShouldUpdatePoll_WhenEditable()
        {
            // Arrange
            var db = GetDbContext(nameof(UpdatePollAsync_ShouldUpdatePoll_WhenEditable));
            var poll = new Poll
            {
                Question = "Q",
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(3),
                Minvotes = 1,
                Maxvotes = 2,
                CreatedByUserId = "user1",
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" }
                }
            };
            db.Polls.Add(poll);
            db.SaveChanges();

            var service = new PollsService(db);

            // Act
            var result = await service.UpdatePollAsync(
                poll.Id,
                "New Q",
                DateTime.UtcNow.AddDays(4),
                DateTime.UtcNow.AddDays(5),
                1,
                2,
                new List<string> { "X", "Y" },
                "user1"
            );

            // Assert
            result.Should().BeTrue();
            var updated = db.Polls.Include(p => p.PollOptions).First(p => p.Id == poll.Id);
            updated.Question.Should().Be("New Q");
            updated.PollOptions.Select(o => o.OptionText).Should().Contain(new[] { "X", "Y" });
        }
    }

}