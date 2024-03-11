using MailKit.Net.Smtp;
using MimeKit;
using User.ManagementSchool.Service.Models;

namespace User.ManagementSchool.Service.Service;

public class EmailService: IEmailService
{
    private readonly EmailConfiguration _emailConfiguration;
    public EmailService(EmailConfiguration emailConfiguration) => _emailConfiguration = emailConfiguration;
    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Email confirmation",_emailConfiguration.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
        return emailMessage;
    }
    private void Send(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();
        try
        {
            client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);
            client.Send(mailMessage);
        }
        catch
        {
            //log an error message or throw an exception, or both.
            throw;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }
}