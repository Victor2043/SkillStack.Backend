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
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var smtpUser = _configuration["EmailSettings:SmtpUser"];
        var smtpPass = _configuration["EmailSettings:SmtpPass"];

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
