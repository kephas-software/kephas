// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;

namespace Kephas.Extensions.Validation;

using ValidationResult = FluentValidation.Results.ValidationResult;

internal class DataAnnotationsValidator : IValidator
{
    public ValidationResult Validate(IValidationContext context)
    {
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (Validator.TryValidateObject(context.InstanceToValidate, new ValidationContext(context.InstanceToValidate), results, true))
        {
            return new ValidationResult();
        }

        return new ValidationResult(results.Select(ToValidationFailure));
    }

    public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken())
    {
        return Task.FromResult(Validate(context));
    }

    private ValidationFailure ToValidationFailure(System.ComponentModel.DataAnnotations.ValidationResult validationResult)
    {
        return new ValidationFailure(
            validationResult.MemberNames?.FirstOrDefault() ?? "instance",
            validationResult.ErrorMessage);
    }

    IValidatorDescriptor IValidator.CreateDescriptor() => null!;

    bool IValidator.CanValidateInstancesOfType(Type type) => true;
}
