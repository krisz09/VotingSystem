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

        public async Task<IReadOnlyCollection<Poll>> GetActivePollsAsync()
        {
            return await _context.Polls
                .Include(p => p.PollOptions)
                .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
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
