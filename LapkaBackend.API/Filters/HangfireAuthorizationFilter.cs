using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hangfire.Dashboard;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.API.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
         var httpContext = context.GetHttpContext();
         
         var token = new JwtSecurityToken(httpContext.Request.Cookies["token"]);
         var role = token.Claims.ToList().
             First(x => x.Type.Equals(ClaimTypes.Role)).Value;
         
         return  role == Roles.SuperAdmin.ToString();
    }
}