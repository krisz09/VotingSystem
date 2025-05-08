using VotingSystem.AdminClient.ViewModels;

namespace VotingSystem.AdminClient.Services
{
    public interface IAuthenticationService
    {
        public Task<string?> GetTokenAsync();
        public Task<bool> LoginAsync(LoginViewModel loginBindingViewModel);
        public Task LogoutAsync();
        public Task<bool> TryAutoLoginAsync();
        public Task<string?> GetCurrentlyLoggedInUserAsync();
    }
}
