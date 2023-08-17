using Hangfire.Dashboard;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.API.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
         var httpContext = context.GetHttpContext();
         
         return httpContext.Request.Cookies["role"] == Roles.SuperAdmin.ToString();
    }
}