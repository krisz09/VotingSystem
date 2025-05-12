using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Services
{
    public interface IPollsService
    {
        public Task<(List<Poll> Polls, List<int> VotedPollIds)> GetActivePollsWithVotesAsync(string userId);
        public Task<List<int>> GetVotedPollIdsForUserAsync(string userId);
        public Task<Poll?> GetPollByIdAsync(int id);
        public Task<bool> SubmitVotesAsync(List<int> pollOptionIds, string userId);
        public Task<IReadOnlyCollection<Poll>> GetAllPollsAsync();
        public Task<IReadOnlyCollection<Poll>> GetClosedPollsAsync(string? questionText, DateTime? startDate, DateTime? endDate);
        public Task<IReadOnlyCollection<Poll>> GetPollsCreatedByUser(string userid);
        public Task<bool> CreatePollAsync(Poll newPoll);

        public Task<bool> UpdatePollAsync
            (int pollId,
            string question,
            DateTime startDate,
            DateTime endDate,
            int minVotes,
            int maxVotes,
            List<string> options,
            string userId);
    }
}
