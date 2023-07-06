using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface ILapkaBackendDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
