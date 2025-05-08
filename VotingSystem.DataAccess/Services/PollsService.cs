using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Services
{
    public class PollsService : IPollsService
    {
        private readonly VotingSystemDbContext _context;

        public PollsService(VotingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Poll>> GetAllPollsAsync()
        {
            return await _context.Polls
                .Include(p => p.PollOptions)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Poll>> GetActivePollsWithVotesAsync(string userId)
        {
            var polls = await _context.Polls
                .Include(p => p.PollOptions)
                .Where(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            var votedPollIds = await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => v.PollOption.PollId)
                .Distinct()
                .ToListAsync();

            return polls;
        }

        public async Task<IReadOnlyCollection<Poll>> GetClosedPollsAsync(string? questionText, DateTime? startDate, DateTime? endDate)
        {
            var closedPollsQuery = _context.Polls
                .Where(p => p.EndDate < DateTime.Now); // Polls that have ended

            if (!string.IsNullOrEmpty(questionText))
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.Question.Contains(questionText));
            }

            if (startDate.HasValue)
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.EndDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                closedPollsQuery = closedPollsQuery.Where(p => p.EndDate <= endDate.Value);
            }

            return await closedPollsQuery.ToListAsync();
        }

        public async Task<IReadOnlyCollection<Poll>> GetPollsCreatedByUser(string userId)
        {
            var polls = await _context.Polls
                .Include(p => p.PollOptions)
                    .ThenInclude(o => o.Votes)
                        .ThenInclude(v => v.User)
                .Where(p => p.CreatedByUserId == userId)
                .ToListAsync();

            return polls;
        }

        public async Task<bool> CreatePollAsync(Poll newPoll)
        {
            _context.Polls.Add(newPoll);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<List<int>> GetVotedPollIdsForUserAsync(string userId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => v.PollOption.PollId)
                .Distinct()
                .ToListAsync();
        }



        public async Task<bool> SubmitVoteAsync(int pollOptionId, string userId)
        {
            var pollOption = await _context.PollOptions
                .Include(po => po.Poll)
                .FirstOrDefaultAsync(po => po.Id == pollOptionId);

            if (pollOption == null || pollOption.Poll.EndDate < DateTime.Now)
            {
                return false;
            }

            var vote = new Vote
            {
                PollOptionId = pollOptionId,
                UserId = userId,
                VotedAt = DateTime.Now
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return true;
        }


    }


}
