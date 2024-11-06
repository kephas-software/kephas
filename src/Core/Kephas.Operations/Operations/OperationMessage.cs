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
    /// Gets the message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public long? Timestamp { get; init; }

    /// <summary>
    /// Gets the severity level.
    /// </summary>
    public SeverityLevel Severity { get; init; } = SeverityLevel.Information;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var time = Timestamp is null ? null : $"{DateTimeOffset.FromUnixTimeMilliseconds(Timestamp.Value):s} ";
        return $"[{time}{Severity}] {Message}";
    }
}