using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<List<User>> AddUser(User user);
        Task<List<User>> UpdateUser(User user, int id);
        Task<List<User>> DeleteUser(int id);
        public Task<User> FindUserByRefreshToken(TokensDto token);

    }
}
