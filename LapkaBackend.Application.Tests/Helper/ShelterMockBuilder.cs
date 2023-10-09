namespace LapkaBackend.Application.Tests.Helper;

public class ShelterMockBuilder
{
    private readonly Shelter _shelter = new ProxyGenerator().CreateClassProxy<Shelter>();

    public ShelterMockBuilder WithCity(string city)
    {
        _shelter.City = city;
        return this;
    }

    public ShelterMockBuilder WithKrs(string krs)
    {
        _shelter.Krs = krs;
        return this;
    }

    public ShelterMockBuilder WithLatitude(double latitude)
    {
        _shelter.Latitude = latitude;
        return this;
    }

    public ShelterMockBuilder WithLongitude(double longitude)
    {
        _shelter.Longitude = longitude;
        return this;
    }

    public ShelterMockBuilder WithNip(string nip)
    {
        _shelter.Nip = nip;
        return this;
    }

    public ShelterMockBuilder WithOrganizationName(string organizationName)
    {
        _shelter.OrganizationName = organizationName;
        return this;
    }
    
    public ShelterMockBuilder WithPhoneNumber(string phoneNumber)
    {
        _shelter.PhoneNumber = phoneNumber;
        return this;
    }
    
    public ShelterMockBuilder WithStreet(string street)
    {
        _shelter.Street = street;
        return this;
    }
    
    public ShelterMockBuilder WithZipCode(string zipCode)
    {
        _shelter.ZipCode = zipCode;
        return this;
    }
    //If more fields are needed add them here

    public Shelter Build()
    {
        return _shelter;
    }
}