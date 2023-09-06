using System.Text.Json.Serialization;
using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Dtos.Responses;

public class FacebookUserResponseDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; }
    
    [JsonPropertyName("first_name")] 
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    
}