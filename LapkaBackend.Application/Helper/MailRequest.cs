using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Helper
{
    public class MailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public Templates Template { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
    }
}
