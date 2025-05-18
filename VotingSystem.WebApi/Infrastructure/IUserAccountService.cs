namespace VotingSystem.WebApi.Infrastructure
{
    public interface IUserAccountService
    {
        public Task<bool> SendResetPasswordLinkAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}