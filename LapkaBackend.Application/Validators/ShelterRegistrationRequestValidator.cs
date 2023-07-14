using FluentValidation;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class ShelterRegistrationRequestValidator : AbstractValidator<ShelterRegistrationRequest>
{
    public ShelterRegistrationRequestValidator()
    {
        RuleFor(x => x.OrganizationName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(64)
            .WithErrorCode("invalid_organization_name"); 
        RuleFor(x => x.Longitude)
            .NotEmpty()
            .WithErrorCode("invalid_longitude");
        RuleFor(x => x.Latitude)
            .NotEmpty()
            .WithErrorCode("invalid_latitude");
        RuleFor(x => x.City)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(64)
            .WithErrorCode("invalid_city");
        RuleFor(x => x.Street)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(64)
            .WithErrorCode("invalid_street");
        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .Matches("^[0-9]{2}-[0-9]{3}$")
            .WithMessage("Zip code is in wrong format")
            .WithErrorCode("invalid_zip_code");
        RuleFor(x => x.Nip)
            .NotEmpty()
            .Matches("^[0-9]{10}$")
            .WithMessage("Nip must be 10 digits number")
            .WithErrorCode("invalid_nip");
        RuleFor(x => x.Krs)
            .NotEmpty()
            .Matches("^[0-9]{10}$")
            .WithMessage("Krs must be 10 digits number")
            .WithErrorCode("invalid_krs");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches("^[0-9]{9}$")
            .WithMessage("Phone number must be 9 digits number")
            .WithErrorCode("invalid_phone_number");
    }
}