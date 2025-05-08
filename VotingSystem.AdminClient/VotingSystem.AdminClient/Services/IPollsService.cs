using VotingSystem.AdminClient.ViewModels;

namespace VotingSystem.AdminClient.Services
{
    public interface IPollsService
    {
        public Task<List<PollViewModel>> GetPollsCreatedByUserAsync();
        public Task<bool> CreatePollAsync(CreatePollViewModel createPollViewModel);
    }
}
