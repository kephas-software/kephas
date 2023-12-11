// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationFailedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Exception for signalling operation failed errors.
/// </summary>
public class OperationFailedException : OperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public OperationFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The inner exception.</param>
    public OperationFailedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
