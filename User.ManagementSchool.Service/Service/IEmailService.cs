using User.ManagementSchool.Service.Models;

namespace User.ManagementSchool.Service.Service;

public interface IEmailService
{
    void SendEmail(Message message);
}