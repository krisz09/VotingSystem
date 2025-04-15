using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Services
{
    public interface IPollsService
    {
        public Task<IReadOnlyCollection<Poll>> GetActivePollsWithVotesAsync(string userId);
        public Task<List<int>> GetVotedPollIdsForUserAsync(string userId);
        public Task<bool> SubmitVoteAsync(int pollOptionId, string userId);
        public Task<IReadOnlyCollection<Poll>> GetAllPollsAsync();
    }
}
