using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmail(MailRequest request);
    }
}
