// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularDependencyException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the circular dependency exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection;

using System;

/// <summary>
/// Exception for signalling circular dependency errors.
/// </summary>
public class CircularDependencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">Optional. The inner exception.</param>
    public CircularDependencyException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}