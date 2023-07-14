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
        Task DeleteUser(Guid id);
        public Task<User> FindUserByRefreshToken(TokenRequest request);
        public Task<User> FindUserByEmail(string email);

    }
}
