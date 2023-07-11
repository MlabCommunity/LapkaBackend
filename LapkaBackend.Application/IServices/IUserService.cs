using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.IServices;

public interface IUserService
{
    public Task<User> LoginMobile(Credentials credentials);
    public Task<bool> LoginWeb(Credentials credentials);
    public Task Register(Credentials credentials);
}