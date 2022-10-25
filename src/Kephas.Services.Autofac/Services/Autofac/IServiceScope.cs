// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Autofac;

/// <summary>
/// Disposable service scope providing the scoped service provider.
/// </summary>
public interface IServiceScope : IDisposable
{
    /// <summary>
    /// Gets the scoped service provider.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}