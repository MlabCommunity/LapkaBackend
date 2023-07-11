using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.IServices;

public interface IUserService
{
    public Task<User> LoginMobile(IDataContext context, Credentials credentials);
    public Task<bool> LoginWeb(IDataContext context, Credentials credentials);
    public Task Register(IDataContext context, Credentials credentials);
}