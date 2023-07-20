using LapkaBackend.Application.Helpter;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmail(MailRequest request);
    }
}
