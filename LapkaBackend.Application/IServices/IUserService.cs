using LapkaBackend.Domain;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.IServices;

public interface IUserService
{
    public bool LoginMobile(IDataContext context, List<String> credentials);
    public bool LoginWeb(IDataContext context, List<String> credentials);
    public bool Register(IDataContext context, List<String> credentials);
    public bool ValidateToken(string token);
    public string GenerateToken(DateTime expDate, string type);
}