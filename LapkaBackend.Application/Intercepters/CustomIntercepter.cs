using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.Application.Intercepters;

public class CustomIntercepter : IValidatorInterceptor
{
    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        return commonContext;
    }

    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext,
        ValidationResult result)
    {
        var failures = result.Errors
            .Select(error => new ValidationFailure(error.PropertyName, SerializeError(error)));

        return new ValidationResult(failures);
    }

    private static string SerializeError(ValidationFailure failure)
    {
        var error = new Error(failure.ErrorCode, failure.ErrorMessage);
        return JsonSerializer.Serialize(error);
    }
}