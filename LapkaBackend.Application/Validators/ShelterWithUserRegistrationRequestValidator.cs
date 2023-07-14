using FluentValidation;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class ShelterWithUserRegistrationRequestValidator : AbstractValidator<ShelterWithUserRegistrationRequest>
{
    public ShelterWithUserRegistrationRequestValidator()
    {
        RuleFor(x => x.UserRequest)
            .SetValidator(new UserRegistrationRequestValidator());
        RuleFor(x => x.ShelterRequest)
            .SetValidator(new ShelterRegistrationRequestValidator());
    }
}