using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class UpdateShelterVolunteeringCommandValidator : AbstractValidator<UpdateShelterVolunteeringRequest>
    {
        public UpdateShelterVolunteeringCommandValidator()
        {
            RuleFor(x => x.BankAccountNumber)
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

