using LapkaBackend.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure
{
    public static class Extensions
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            //services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ILapkaBackendDbContext, LapkaBackendDbContext>();
            services.AddDbContext<LapkaBackendDbContext>();
        }
    }
}
