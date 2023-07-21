using LapkaBackend.Domain.Enums;
using MimeKit;

namespace LapkaBackend.Application.Interfaces
{
    public interface IEmailWrapper
    {
        BodyBuilder GetBuilder(Templates template);
    }
}
