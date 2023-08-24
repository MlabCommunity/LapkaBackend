using FluentValidation;
using LapkaBackend.Application.Functions.Command;

namespace LapkaBackend.Application.Validators;

    public class ShelterVolunteeringRequestValidator : AbstractValidator<UpdateShelterVolunteeringCommand>
    {
        public ShelterVolunteeringRequestValidator()
        {
            RuleFor(x => x.ShelterId)
                .NotEmpty();
            RuleFor(x => x.BankAccountNumber)
                .NotEmpty()
                .Matches(@"^\d{26}$")
                .WithErrorCode("invalid_AccountNumber")
                .WithMessage("In Poland Required 26 characters.");
        }
    }

