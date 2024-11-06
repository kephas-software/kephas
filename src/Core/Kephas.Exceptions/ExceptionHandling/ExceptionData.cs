// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling;

using System;

/// <summary>
/// An exception data.
/// </summary>
public record ExceptionData() : IErrorNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionData"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="severity">Optional. The severity level. If this value is set, it overwrites the severity level provided by the exception.</param>
    public ExceptionData(Exception exception, SeverityLevel? severity = null)
        : this()
    {
        ArgumentNullException.ThrowIfNull(exception);

        var errorNotification = exception as IErrorNotification;
        severity ??= errorNotification?.Severity ?? SeverityLevel.Error;

        ErrorType = errorNotification?.ErrorType ?? exception.GetType().Name;
        Message = exception.Message;
        Severity = severity.Value;
    }

    /// <summary>
    /// Gets or sets the type of the error.
    /// </summary>
    public string? ErrorType { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    /// <value>
    /// The severity level.
    /// </value>
    public SeverityLevel Severity { get; set; } = SeverityLevel.Error;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        $"{Message} ({ErrorType}: {Severity})";
}