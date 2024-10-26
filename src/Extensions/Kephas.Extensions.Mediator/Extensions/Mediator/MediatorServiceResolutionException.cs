// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceResolutionException.cs" company="Hays AG">
//   Copyright (c) Hays AG. All rights reserved.
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
