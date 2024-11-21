using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using User.ManagementSchool.Service.Models;

namespace User.ManagementSchool.Service.Service;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfiguration;

    public EmailService(EmailConfiguration emailConfiguration)
    {
        _emailConfiguration = emailConfiguration;
    }

    public async Task SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        await SendAsync(emailMessage); 
    }
    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Email Confirmation", _emailConfiguration.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;

        emailMessage.Body = message.IsHtml
            ? new TextPart(TextFormat.Html) { Text = message.Content }
            : new TextPart(TextFormat.Text) { Text = message.Content };

        return emailMessage;
    }


    private async Task SendAsync(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true); 
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password); 
            await client.SendAsync(mailMessage);
        }
        finally
        {
            await client.DisconnectAsync(true); 
            client.Dispose();
        }
    }

}