﻿using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Common
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
