using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class ShelterVolunteeringRequestValidator : AbstractValidator<UpdateShelterVolunteeringRequest>
    {
        public ShelterVolunteeringRequestValidator()
        {
            RuleFor(x => x.BankAccountNumber)
                .Must(accountNumber => accountNumber == null || Regex.IsMatch(accountNumber, @"^\d{26}$"))
                .WithErrorCode("invalid_AccountNumber")
                .WithMessage("In Poland Required 26 characters.");
            RuleFor(x => x.DonationDescription)
                .MaximumLength(150)
                .WithErrorCode("invalid_donation_description");
            RuleFor(x => x.DailyHelpDescription)
                .MaximumLength(150)
                .WithErrorCode("invalid_daily_help_description");
            RuleFor(x => x.TakingDogsOutDescription)
                .MaximumLength(150)
                .WithErrorCode("invalid_last_name");
        }
    }

