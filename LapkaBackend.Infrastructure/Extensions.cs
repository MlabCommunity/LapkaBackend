using LapkaBackend.Application.Common;
using LapkaBackend.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

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
