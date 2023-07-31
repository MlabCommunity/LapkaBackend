using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<GetUserDataByIdQueryResult> GetUserById(Guid id);
        Task<User> AddUser(User user);
        Task UpdateUser(UpdateUserDataRequest request, string id);
        Task DeleteUser(string id);
        Task<User> FindUserByRefreshToken(TokenRequest request);
        Task<User> FindUserByEmail(string email);
        Task SetNewPassword(string id, UserPasswordRequest request);
        Task SetNewEmail(string id, UpdateUserEmailRequest request);
        Task<GetCurrentUserDataQueryResult> GetLoggedUser(string id);
        Task VerifyEmail(string token);
        Task DeleteProfilePicture(string id);

    }
}
