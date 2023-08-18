using Hangfire;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Infrastructure.Data;
using LapkaBackend.Infrastructure.Email;
using LapkaBackend.Infrastructure.FileStorage;
using LapkaBackend.Infrastructure.Hangfire;
using LapkaBackend.Infrastructure.ModelBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LapkaBackend.Infrastructure
{
    public static class Extensions
    {

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDataContext, DataContext>();
            services.AddTransient<IAzureStorageContext, AzureStorageContext>();
            services.AddTransient<IEmailWrapper, EmailWrapper>();
            services.AddTransient<UpdateDeleteJob>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MySql"));
            });
            
            services.AddHangfire(options => options
                .UseSqlServerStorage(configuration.GetConnectionString("MySql")));

            services.AddHangfireServer();
        }

        public static void AddModels(this ModelBuilder modelBuilder)
        {
            UserModelBuilder.BuildUserModel(modelBuilder);
            ShelterModelBuilder.BuildShelterModel(modelBuilder);
            RoleModelBuilder.BuildRoleModel(modelBuilder);
            AnimalModelBuilder.BuildAnimalModel(modelBuilder);
            AnimalCategoryModelBuilder.BuildAnimalCategoryModel(modelBuilder);
            ReactionModelBuilder.BuildReactionModel(modelBuilder);
            AnimalViewsModelBuilder.BuildAnimalViewModel(modelBuilder);
            ShelterVolunteeringModelBuilder.BuildShelterVolunteeringModel(modelBuilder);
        }

        public static void Seed(DbContextOptions<DataContext> options)
        {
            AnimalCategoryModelBuilder.SeedAnimalCategories(options);
            RoleModelBuilder.SeedRoles(options);
            UserModelBuilder.SeedUser(options);
        }
    }
}
