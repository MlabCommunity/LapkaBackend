using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(MailRequest request);
    }
}
