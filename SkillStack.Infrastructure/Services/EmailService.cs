using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SkillStack.Core.Interfaces;

namespace SkillStack.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
        var smtpUser = _configuration["EmailSettings:SenderEmail"];
        var smtpPass = _configuration["EmailSettings:Password"];


        using var client = new SmtpClient(smtpHost, smtpPort)
        {

            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(smtpUser, to, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}
