using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Interfaces
{
    public interface IManagementService
    {
        public Task AssignAdminRole(Guid userId);
        public Task RemoveAdminRole(Guid userId);
        public Task<GetUsersByRoleQueryResult> ListOfUsersWithTheSpecifiedRole(Roles role);
    }
}
