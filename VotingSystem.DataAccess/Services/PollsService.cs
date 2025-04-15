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
