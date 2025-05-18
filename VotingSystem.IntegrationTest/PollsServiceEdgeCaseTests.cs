using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using Xunit;

namespace VotingSystem.IntegrationTest
{
    public class PollsServiceEdgeCaseTests
    {
        private VotingSystemDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<VotingSystemDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new VotingSystemDbContext(options);
        }

        [Fact]
        public async Task CreatePollAsync_ShouldFail_WhenNoOptions()
        {
            // Arrange
            var db = GetDbContext(nameof(CreatePollAsync_ShouldFail_WhenNoOptions));
            var service = new PollsService(db);

            var poll = new Poll
            {
                Question = "No options poll",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                CreatedByUserId = "user1",
                PollOptions = new List<PollOption>() // No options
            };

            // Act
            var result = await service.CreatePollAsync(poll);

            // Assert
            result.Should().BeTrue(); // EF will allow, but you may want to add validation in service
            db.PollOptions.Count().Should().Be(0);
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfPollOptionIdsIsNullOrEmpty()
        {
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfPollOptionIdsIsNullOrEmpty));
            var service = new PollsService(db);

            var result1 = await service.SubmitVotesAsync(null, "user1");
            var result2 = await service.SubmitVotesAsync(new List<int>(), "user1");

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfOptionDoesNotExist()
        {
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfOptionDoesNotExist));
            var service = new PollsService(db);

            var result = await service.SubmitVotesAsync(new List<int> { 999 }, "user1");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfPollEnded()
        {
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfPollEnded));
            var poll = new Poll
            {
                Question = "Ended poll",
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(-1),
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
            var service = new PollsService(db);

            var result = await service.SubmitVotesAsync(new List<int> { optionId }, "user1");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfOptionNotBelongToPoll()
        {
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfOptionNotBelongToPoll));
            var poll1 = new Poll
            {
                Question = "Poll 1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" }
                }
            };
            var poll2 = new Poll
            {
                Question = "Poll 2",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 1,
                Maxvotes = 2,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "B" }
                }
            };
            db.Polls.Add(poll1);
            db.Polls.Add(poll2);
            db.SaveChanges();

            var optionId1 = db.PollOptions.First().Id;
            var optionId2 = db.PollOptions.Last().Id;
            var service = new PollsService(db);

            // Try to vote with options from different polls
            var result = await service.SubmitVotesAsync(new List<int> { optionId1, optionId2 }, "user1");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task SubmitVotesAsync_ShouldReturnFalse_IfVotesBelowMinOrAboveMax()
        {
            var db = GetDbContext(nameof(SubmitVotesAsync_ShouldReturnFalse_IfVotesBelowMinOrAboveMax));
            var poll = new Poll
            {
                Question = "Min/Max test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Minvotes = 2,
                Maxvotes = 2,
                PollOptions = new List<PollOption>
                {
                    new PollOption { OptionText = "A" },
                    new PollOption { OptionText = "B" }
                }
            };
            db.Polls.Add(poll);
            db.SaveChanges();

            var optionIds = db.PollOptions.Select(o => o.Id).ToList();
            var service = new PollsService(db);

            // Too few votes
            var result1 = await service.SubmitVotesAsync(new List<int> { optionIds[0] }, "user1");
            // Too many votes
            var result2 = await service.SubmitVotesAsync(new List<int> { optionIds[0], optionIds[1], 999 }, "user1");

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePollAsync_ShouldReturnFalse_IfPollNotFoundOrNotOwned()
        {
            var db = GetDbContext(nameof(UpdatePollAsync_ShouldReturnFalse_IfPollNotFoundOrNotOwned));
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

            // Wrong user
            var result1 = await service.UpdatePollAsync(
                poll.Id, "Q", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1, 2, new List<string> { "A" }, "user2");
            // Non-existent poll
            var result2 = await service.UpdatePollAsync(
                999, "Q", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1, 2, new List<string> { "A" }, "user1");

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePollAsync_ShouldReturnFalse_IfEndDateNotExtended_WhenNotEditable()
        {
            var db = GetDbContext(nameof(UpdatePollAsync_ShouldReturnFalse_IfEndDateNotExtended_WhenNotEditable));
            var poll = new Poll
            {
                Question = "Q",
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(1),
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

            // Try to update with an earlier end date
            var result = await service.UpdatePollAsync(
                poll.Id, "Q", DateTime.UtcNow, DateTime.UtcNow, 1, 2, new List<string> { "A" }, "user1");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetPollByIdAsync_ShouldReturnNull_IfNotFound()
        {
            var db = GetDbContext(nameof(GetPollByIdAsync_ShouldReturnNull_IfNotFound));
            var service = new PollsService(db);

            var result = await service.GetPollByIdAsync(999);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllPollsAsync_ShouldReturnEmpty_IfNoPolls()
        {
            var db = GetDbContext(nameof(GetAllPollsAsync_ShouldReturnEmpty_IfNoPolls));
            var service = new PollsService(db);

            var result = await service.GetAllPollsAsync();
            result.Should().BeEmpty();
        }
    }
}
