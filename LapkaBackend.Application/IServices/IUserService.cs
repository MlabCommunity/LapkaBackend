using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.IServices;

public interface IUserService
{
    public Task<User> LoginMobile(IDataContext context, Dictionary<string, string> credentials);
    public Task<bool> LoginWeb(IDataContext context, Dictionary<string, string> credentials);
    public Task Register(IDataContext context, Dictionary<string, string> credentials);
}