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
    public async Task<ActionResult> GetFile(Guid id)
    {
        return Ok(await _blobService.GetFileUrlAsync(id));
    }
    
    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFile(IFormFile file)
    {
        var result = await _blobService.UploadFileAsync(file);

        return Ok(result);
    }

    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteFile(Guid id)
    {
        await _blobService.DeleteFileAsync(id);

        return NoContent();
    }
}