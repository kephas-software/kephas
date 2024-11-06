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
    private ExceptionData? exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationMessage"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public ExceptionMessage(Exception exception)
        : this(new ExceptionData(exception ?? throw new ArgumentNullException(nameof(exception))))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationMessage"/> class.
    /// </summary>
    /// <param name="exceptionData">The exception data.</param>
    public ExceptionMessage(ExceptionData exceptionData)
        : this()
    {
        Exception = exceptionData ?? throw new ArgumentNullException(nameof(exceptionData));
        Message = exceptionData.Message;
    }

    /// <summary>
    /// Gets the exception, if any.
    /// </summary>
    public ExceptionData Exception
    {
        get => exception ??= new ExceptionData(new Exception());
        init
        {
            exception = value ?? throw new ArgumentNullException(nameof(value));
            Severity = exception!.Severity;
        }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() =>
        $"{base.ToString()} [{exception!.ExceptionType}]";
}