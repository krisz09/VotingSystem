namespace VotingSystem.WebApi.Infrastructure
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body); 
    }
}
