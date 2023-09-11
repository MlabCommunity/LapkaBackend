using System.ComponentModel;
using System.Text.Json.Serialization;
using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Dtos.Responses;

public class GoogleUserResponseDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; }

    [JsonPropertyName("email_verified")]
    [JsonConverter(typeof(BoolConverter))]
    public bool EmailVerified{ get; init; }

    [JsonPropertyName("given_name")]
    public string FirstName { get; init; }    
    
    [JsonPropertyName("family_name")]
    public string LastName { get; init; }     
    
    [JsonPropertyName("name")]
    public string Name { get; init; }    
    
    [JsonPropertyName("picture")]
    public string ProfilePicture { get; init; }    
    
    [JsonPropertyName("sub")]
    public string ExternalId { get; init; }
}