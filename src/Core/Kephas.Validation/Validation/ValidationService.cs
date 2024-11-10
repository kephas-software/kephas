// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;
using Kephas.DependencyInjection;

namespace Kephas.Validation;

/// <summary>
/// Default implementation of the <see cref="IValidationService"/>.
/// </summary>
public class ValidationService(ILazyEnumerable<IValidator, ValidatorMetadata> lazyEnumerable) : IValidationService
{
    /// <summary>Validates the specified instance.</summary>
    /// <param name="obj">The object to be validated.</param>
    /// <returns>A ValidationResult object contains any validation failures.</returns>
    public ValidationResult Validate(object? obj)
    {
        if (obj is null) return new ValidationResult();

        var validatedType = obj.GetType();
        var validators = GetValidators(validatedType);
        return new ValidationResult(validators.Select(v => v.Validate(GetValidationContext(obj, validatedType))));
    }

    /// <summary>Validates the specified instance asynchronously.</summary>
    /// <param name="obj">The object to be validated.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A ValidationResult object contains any validation failures.</returns>
    public async Task<ValidationResult> ValidateAsync(object? obj, CancellationToken cancellationToken = default)
    {
        if (obj is null) return new ValidationResult();

        var validatedType = obj.GetType();
        var validators = GetValidators(validatedType);
        var results = new List<ValidationResult>();
        foreach (var validator in validators)
        {
            results.Add(await validator.ValidateAsync(GetValidationContext(obj, validatedType), cancellationToken));
        }

        return new ValidationResult(results);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<IValidator> GetValidators(Type validatedType) =>
        lazyEnumerable.SelectServices(m => m.ValidatedType?.IsAssignableFrom(validatedType) is true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IValidationContext GetValidationContext(object obj, Type objType) =>
        (IValidationContext)Activator.CreateInstance(typeof(ValidationContext<>).MakeGenericType(objType), obj)!;
}
