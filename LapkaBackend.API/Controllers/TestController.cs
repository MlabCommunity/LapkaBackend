using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IDataContext _context;

    public TestController(IConfiguration configuration, IDataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpGet("api/test/db-health-check")]
    public async Task<IActionResult> DataBaseHealthCheck()
    {
        var role = new Role
        {
            Id = -1,
            RoleName = "test"
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok();
    }
}