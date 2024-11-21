using User.ManagementSchool.Service.Models;

namespace User.ManagementSchool.Service.Service;

public interface IEmailService
{
    Task SendEmailAsync(Message message);
}