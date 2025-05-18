using Microsoft.AspNetCore.Identity;
using System.Web;
using VotingSystem.DataAccess;
using VotingSystem.WebApi.Infrastructure;

namespace VotingSystem.WebApi.Infrastructure
{

    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public UserAccountService(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<bool> SendResetPasswordLinkAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = $"http://localhost:3000/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(email)}";
            await _emailService.SendEmailAsync(email, "Reset your password", $"Click <a href='{url}'>here</a>.");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

    }

}