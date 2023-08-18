using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

