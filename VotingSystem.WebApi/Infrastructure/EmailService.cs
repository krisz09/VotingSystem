using MailKit.Net.Smtp;
using MimeKit;
using VotingSystem.WebApi.Infrastructure;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Voting System", _configuration["EmailSettings:From"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["EmailSettings:SmtpHost"], 587, false);
        await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
