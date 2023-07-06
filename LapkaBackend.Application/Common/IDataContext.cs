using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Common
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Auth> Auths { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public EntityEntry Entry(object entity);
    }
}
