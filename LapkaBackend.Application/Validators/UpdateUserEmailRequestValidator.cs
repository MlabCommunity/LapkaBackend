using FluentValidation;
using LapkaBackend.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
    {
        public UpdateUserEmailRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("invalid_email");
        }
    }

