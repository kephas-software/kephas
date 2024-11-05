// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceResolutionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Mediator exception during service resolution.
/// </summary>
public class MediatorServiceResolutionException : ServiceResolutionException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorServiceResolutionException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public MediatorServiceResolutionException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorServiceResolutionException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The inner exception.</param>
    public MediatorServiceResolutionException(string message, Exception inner) : base(message, inner)
    {
    }
}
