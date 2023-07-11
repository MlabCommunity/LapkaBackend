using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application
{
    public static class Extensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            //services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAuthService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var dbContext = provider.GetRequiredService<ILapkaBackendDbContext>();
                var secretKey = configuration["JwtConfig:SecretKey"];
                return new AuthService(configuration, dbContext, secretKey);
            });

        }
    }
}
