using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Data;
using LapkaBackend.Infrastructure.ModelBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LapkaBackend.Infrastructure
{
    public static class Extensions
    {

        public static void AddInfrasturcture(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDataContext, DataContext>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MySql"));
            });
        }

        public static void AddModels(this ModelBuilder modelBuilder)
        {
            UserModelBuilder.BuildUserModel(modelBuilder);
            ShelterModelBuilder.BuildShelterModel(modelBuilder);
            RoleModelBuilder.BuildRoleModel(modelBuilder);
        }

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                    new Role() { Id =  1, RoleName = "SuperAdmin" },
                    new Role() { Id =  2, RoleName = "Shelter" },
                    new Role() { Id =  3, RoleName = "User" },
                    new Role() { Id =  4, RoleName = "Worker" }
                );
        }
    }
}
