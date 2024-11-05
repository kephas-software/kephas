// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediatorServiceInfosResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Resolves an ordered list of handlers matching the requested handler type. 
/// </summary>
[SingletonService]
public interface IMediatorHandlerResolver
{
    /// <summary>
    /// Gets an ordered list of handlers matching the requested handler type.
    /// </summary>
    /// <returns>An ordered list of matching handlers.</returns>
    IEnumerable<THandler>? TryGetMatchingHandlerEnumeration<THandler>(IServiceProvider serviceProvider)
        where THandler : notnull;

    /// <summary>
    /// Tries to get a handler matching the requested handler type.
    /// </summary>
    /// <returns>An matching handler or null, if the handler type is not handled.</returns>
    THandler? TryGetMatchingHandler<THandler>(IServiceProvider serviceProvider)
        where THandler : notnull;
}