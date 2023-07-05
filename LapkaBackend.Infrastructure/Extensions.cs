using LapkaBackend.Application.Common;
using LapkaBackend.Infrastructure.Data;
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
        public static void AddInfrasturcture(this IServiceCollection services)
        {
            services.AddTransient<IDataContext, DataContext>();
            services.AddDbContext<DataContext>();
        }
    }
}
