using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Data;
using LapkaBackend.Infrastructure.Email;
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
            services.AddTransient<IEmailWrapper, EmailWrapper>();
            

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
            AnimalModelBuilder.BuildAnimalModel(modelBuilder);
            AnimalCategoryModelBuilder.BuildAnimalCategoryModel(modelBuilder);
        }

        public static void Seed(this ModelBuilder modelBuilder)
        {
            //TODO: Migracja do zrobienia :)
            modelBuilder.Entity<Role>().HasData(
                    new Role() { Id =  1, RoleName = "Undefined" },
                    new Role() { Id =  2, RoleName = "SuperAdmin" },
                    new Role() { Id =  3, RoleName = "Admin" },
                    new Role() { Id =  4, RoleName = "User" },
                    new Role() { Id =  5, RoleName = "Shelter" },
                    new Role() { Id =  6, RoleName = "Worker" }
                );

            modelBuilder.Entity<AnimalCategory>().HasData(
                    new AnimalCategory() { Id = 1, CategoryName = "Dog" },
                    new AnimalCategory() { Id = 2, CategoryName = "Cat" },
                    new AnimalCategory() { Id = 3, CategoryName = "rabbit" }
                );
        }
    }
}
