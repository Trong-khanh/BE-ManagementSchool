using MimeKit;

namespace User.ManagementSchool.Service.Models;

public class Message
{
    public Message(IEnumerable<string> to, string subject, string content, bool isHtml = false)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress("mail", x)));
        Subject = subject;
        Content = content;
        IsHtml = isHtml;
    }

    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool IsHtml { get; set; }
}
