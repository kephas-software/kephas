// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Operations;

/// <summary>
/// An operation message.
/// </summary>
public record OperationMessage() : IOperationMessage
{
    private readonly Exception? exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationMessage"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public OperationMessage(string message)
        : this()
    {
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationMessage"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public OperationMessage(Exception exception)
        : this((exception ?? throw new ArgumentNullException(nameof(exception))).Message)
    {
        Exception = exception;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the exception, if any.
    /// </summary>
    public Exception? Exception
    {
        get => exception;
        init
        {
            exception = value;
            if (exception is IHasSeverityLevel logLevelException)
            {
                SeverityLevel = logLevelException.SeverityLevel;
            }
        }
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public long? Timestamp { get; init; }

    /// <summary>
    /// Gets the severity level.
    /// </summary>
    public SeverityLevel SeverityLevel { get; init; } = SeverityLevel.Information;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var exceptionMessage = Exception is null ? null : $" ({Exception.GetType().Name})";
        var time = Timestamp is null ? null : $"{new DateTimeOffset(new DateTime(Timestamp.Value, DateTimeKind.Utc)):s} ";
        return $"[{time}{SeverityLevel}] {Message}{exceptionMessage}";
    }
}