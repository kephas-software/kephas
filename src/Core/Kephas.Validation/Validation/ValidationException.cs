// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation.Results;
using Kephas.ExceptionHandling;

namespace Kephas.Validation;

/// <summary>
/// Exception occurring when the provided input is not valid.
/// </summary>
public class ValidationException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="validatedInstance">The validated instance.</param>
    /// <param name="validationResult">The validation result.</param>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValidationException(
        object? validatedInstance = null,
        ValidationResult? validationResult = null,
        string? message = null,
        Exception? innerException = null)
        : base(message ?? "The provided input is not valid. Please check the validation result for more information.", innerException)
    {
        ValidatedInstance = validatedInstance;
        ValidationResult = validationResult;
    }

    /// <summary>
    /// Gets the validated instance.
    /// </summary>
    public object? ValidatedInstance { get; }

    /// <summary>
    /// Gets the validation result.
    /// </summary>
    public ValidationResult? ValidationResult { get; }
}
