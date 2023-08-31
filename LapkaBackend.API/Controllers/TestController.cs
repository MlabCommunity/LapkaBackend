using Azure.Storage;
using Azure.Storage.Blobs;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IDataContext _context;
    private readonly IChatService _chatService;

    public TestController(IConfiguration configuration, IDataContext context, IChatService chatService)
    {
        _configuration = configuration;
        _context = context;
        _chatService = chatService;
    }
    
    [HttpGet("api/test/db-health-check")]
    public async Task<IActionResult> DataBaseHealthCheck()
    {
        var role = new Role
        {
            RoleName = "test"
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("api/test/storage-health-check")]
    public async Task<IActionResult> StorageHealthCheck()
    {
        var sharedKeyCredential = new StorageSharedKeyCredential(
            _configuration["Storage:Name"],
            _configuration["Storage:Key"]);
        
        var containerEndpoint = $"https://{_configuration["Storage:Name"]}.blob.core.windows.net/test";
        
        var container = new BlobContainerClient(new Uri(containerEndpoint), sharedKeyCredential);

        return Ok(await container.ExistsAsync());
    }
    
    [HttpGet("api/test/set-env")]
    public async Task<IActionResult> SetEnv()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        return Ok();
    }
}
