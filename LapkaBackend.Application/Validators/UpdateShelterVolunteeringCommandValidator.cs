using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class UpdateShelterVolunteeringCommandValidator : AbstractValidator<UpdateShelterVolunteeringCommand>
    {
        public UpdateShelterVolunteeringCommandValidator()
        {
            RuleFor(x => x.ShelterId)
                .NotEmpty()
                .Length(32)
                .WithMessage("Invalid Id");
            RuleFor(x => x.BankAccountNumber)
                .NotEmpty()
                .Matches(@"^\d{26}$")
                .WithErrorCode("invalid_AccountNumber")
                .WithMessage("In Poland Required 26 characters.");
            RuleFor(x => x.DonationDescription)
                .MaximumLength(150);
            RuleFor(x => x.DailyHelpDescription)
                .MaximumLength(150);
            RuleFor(x => x.TakingDogsOutDescription)
                .MaximumLength(150);
        }
    }

