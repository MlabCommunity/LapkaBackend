using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(Guid id);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user, Guid id);
        Task DeleteUser(string id);
        Task<User> FindUserByRefreshToken(TokenRequest request);
        Task<User> FindUserByEmail(string email);
        Task SetNewPassword(string id, UserPasswordRequest request);
        Task SetNewEmail(string id, UpdateUserEmailRequest request);
        Task<User> GetLoggedUser(string id);
        Task VerifyEmail(string token);

    }
}
