// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperationMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Operations;

/// <summary>
/// Contract for data operation messages.
/// </summary>
public interface IOperationMessage : IHasSeverityLevel
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    string? Message { get; }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    long? Timestamp { get; }
}