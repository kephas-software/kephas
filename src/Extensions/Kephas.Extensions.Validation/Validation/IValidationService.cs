// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation.Results;
using Kephas.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Validation;

/// <summary>
/// Service used for validation.
/// </summary>
[ScopedService]
public interface IValidationService
{
    /// <summary>Validates the specified instance.</summary>
    /// <param name="obj">The object to be validated.</param>
    /// <returns>A ValidationResult object contains any validation failures.</returns>
    ValidationResult Validate(object? obj);

    /// <summary>Validates the specified instance asynchronously.</summary>
    /// <param name="obj">The object to be validated.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A ValidationResult object contains any validation failures.</returns>
    Task<ValidationResult> ValidateAsync(object? obj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures that the provided instance is valid, throwing a <see cref="ValidationException"/> in case of a failed validation failed.
    /// </summary>
    /// <param name="obj">The validated instance.</param>
    void EnsureValid(object? obj)
    {
        var result = Validate(obj);
        if (!result.IsValid)
        {
            throw new ValidationException(obj, result);
        }
    }

    /// <summary>
    /// Ensures that the provided instance is valid, throwing a <see cref="ValidationException"/> in case of a failed async validation failed.
    /// </summary>
    /// <param name="obj">The validated instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    async Task EnsureValidAsync(object? obj, CancellationToken cancellationToken = default)
    {
        var result = await ValidateAsync(obj, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(obj, result);
        }
    }
}
