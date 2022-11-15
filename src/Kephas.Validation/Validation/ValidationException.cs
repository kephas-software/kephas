// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data validation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Validation;

using Kephas.Data;
using Kephas.Resources;

/// <summary>
/// Base class for validation exceptions.
/// </summary>
public class ValidationException : DataException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="validatedEntity">The validated entity.</param>
    /// <param name="validationResult">The validation result.</param>
    public ValidationException(object validatedEntity, IValidationResult validationResult)
        : base(string.Format(ValidationStrings.DataValidationException_Message, validatedEntity, string.Join(Environment.NewLine, validationResult.Select(e => e.Message))))
    {
        this.ValidatedEntity = validatedEntity;
        this.ValidationResult = validationResult;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    protected ValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="inner">
    /// The inner.
    /// </param>
    protected ValidationException(string message, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// Gets the validated entity.
    /// </summary>
    /// <value>
    /// The validated entity.
    /// </value>
    public object? ValidatedEntity { get; }

    /// <summary>
    /// Gets the validation result.
    /// </summary>
    /// <value>
    /// The validation result.
    /// </value>
    public IValidationResult? ValidationResult { get; }
}