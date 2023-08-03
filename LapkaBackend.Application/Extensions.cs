using AutoMapper;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LapkaBackend.Application
{
    public static class Extensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IManagementService, ManagementService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBlobService, BlobService>();

        }
    }
}
