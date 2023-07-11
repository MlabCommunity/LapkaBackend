﻿using LapkaBackend.Application.Common;
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
        }
    }
}
