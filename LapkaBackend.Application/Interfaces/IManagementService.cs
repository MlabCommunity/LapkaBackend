using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Interfaces
{
    public interface IManagementService
    {
        Task AssignAdminRole(Guid userId);
        Task RemoveAdminRole(Guid userId);
        Task<GetUsersByRoleQueryResult> ListOfUsersWithTheSpecifiedRole(Roles role);
    }
}
