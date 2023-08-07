using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<GetUserDataByIdQueryResult> GetUserById(Guid id);
        Task UpdateUser(UpdateUserDataRequest request, Guid id);
        Task DeleteUser(Guid id);
        Task SetNewPassword(Guid id, UserPasswordRequest request);
        Task SetNewEmail(Guid id, UpdateUserEmailRequest request);
        Task<GetCurrentUserDataQueryResult> GetLoggedUser(Guid id);
        Task VerifyEmail(string token);
        Task DeleteProfilePicture(Guid id);

    }
}
