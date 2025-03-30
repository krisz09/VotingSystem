using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Services
{
    public interface IPollsService
    {
        public Task<IReadOnlyCollection<Poll>> GetActivePollsAsync();
        public Task<bool> SubmitVoteAsync(int pollOptionId, string userId);
    }
}
