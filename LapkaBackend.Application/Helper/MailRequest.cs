using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Helper
{
    public class MailRequest
    {
        public string ToEmail { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public Templates Template { get; init; }
        public string RedirectUrl { get; init; } = string.Empty;
    }
}
