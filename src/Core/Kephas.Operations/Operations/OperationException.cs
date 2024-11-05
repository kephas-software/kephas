// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Operations;

/// <summary>
/// Exception for signaling operation errors.
/// </summary>
public class OperationException : Exception, IErrorNotification, IRetryable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public OperationException(string message)
        : base(message)
    {
        Timestamp = DateTimeOffset.Now;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public OperationException(string message, Exception inner)
        : base(message, inner)
    {
        Timestamp = DateTimeOffset.Now;
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <value>
    /// The timestamp.
    /// </value>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets or sets the severity.
    /// </summary>
    /// <value>
    /// The severity.
    /// </value>
    public SeverityLevel Severity { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the operation that generated the exception can be retried.
    /// </summary>
    public bool CanRetry { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation that generated the exception can be retried.
    /// </summary>
    bool IRetryable.CanRetry() => CanRetry;

    /// <summary>Creates and returns a string representation of the current exception.</summary>
    /// <returns>A string representation of the current exception.</returns>
    public override string ToString()
    {
        return $"{Timestamp:s}/{Severity} {base.ToString()}";
    }
}