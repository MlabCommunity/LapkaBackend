using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IManagementService
    {
        public Task AssignRemoveAdminRole(Guid userId, string newRole);
        public Task<List<UserDto>> ListOfUsersWithTheSpecifiedRole(string role);
    }
}
