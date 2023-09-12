﻿using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Interfaces
{
    public interface IManagementService
    {
        public Task AssignAdminRole(Guid userId);
        public Task RemoveAdminRole(Guid userId);
        public Task<GetUsersByRoleQueryResult> ListOfUsersWithTheSpecifiedRole(Roles role);
        public Task AddWorkerByAdmin(string userId);
        public Task RemoveWorkerByAdmin(string userId);
        public Task RemoveWorkerByShelter(Guid userId, Guid shelterId);
        public Task<GetUsersByRoleQueryResult> ListOfWorkersInShelter(Guid shelterId);
    }
}
