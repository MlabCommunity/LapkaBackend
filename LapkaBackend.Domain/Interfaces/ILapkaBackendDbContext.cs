using LapkaBackend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Common
{
    public interface ILapkaBackendDbContext: IDbContext
    {
        DbSet<User> Users { get; set; }
    }
}
