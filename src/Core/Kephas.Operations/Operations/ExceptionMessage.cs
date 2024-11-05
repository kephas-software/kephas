// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Operations;

/// <summary>
/// An operation message based on an exception.
/// </summary>
public record ExceptionMessage() : OperationMessage()
{
    private readonly Exception? exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationMessage"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public ExceptionMessage(Exception exception)
        : this()
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        Message = exception.Message;
    }

    /// <summary>
    /// Gets the exception, if any.
    /// </summary>
    public Exception Exception
    {
        get => exception ?? new Exception();
        init
        {
            exception = value ?? throw new ArgumentNullException(nameof(value));
            if (exception is IHasSeverityLevel logLevelException)
            {
                SeverityLevel = logLevelException.SeverityLevel;
            }
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() =>
        $"{base.ToString()} [{exception?.GetType().Name}]";
}