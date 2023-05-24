// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using Kephas.Dynamic;

/// <summary>
/// Base contract for contexts.
/// </summary>
public interface IContextBase : IDynamic, IDisposable
{
    /// <summary>
    /// Gets a context for the dependency injection/composition.
    /// </summary>
    /// <value>
    /// The injector.
    /// </value>
    IServiceProvider ServiceProvider { get; }
}