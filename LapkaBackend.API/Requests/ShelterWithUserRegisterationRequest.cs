namespace LapkaBackend.API.Requests;

public class ShelterWithUserRegistrationRequest
{
    public ShelterRegistrationRequest Shelter { get; set; }
    public UserRegistrationRequest User { get; set; }
}