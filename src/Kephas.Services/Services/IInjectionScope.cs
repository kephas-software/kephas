// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// Defines an injection scope.
/// </summary>
public interface IInjectionScope : IDisposable
{
    /// <summary>
    /// Gets the service provider for this scope.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }
}