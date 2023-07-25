using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class StorageController : Controller
{
    private readonly IBlobService _blobService;

    public StorageController(IBlobService blobService)
    {
        _blobService = blobService;
    }
    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpGet ("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFile(string id)
    {
        return Ok(_blobService.GetFileUrlAsync(id));
    }
    
    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpPost ("get")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetFile()
    {
        
        return NoContent();
    }
}