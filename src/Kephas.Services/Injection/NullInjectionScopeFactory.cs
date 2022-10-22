// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullInjectionScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using Kephas.Services;

/// <summary>
/// Injection scope factory returning a scope
/// </summary>
[OverridePriority(Priority.Lowest)]
public class NullInjectionScopeFactory : IInjectionScopeFactory
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullInjectionScopeFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public NullInjectionScopeFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a disposable injection scope.
    /// </summary>
    /// <returns>
    /// The new disposable injection scope.
    /// </returns>
    public IInjectionScope CreateScope() => new InjectionScope(this.serviceProvider, ownsServiceProvider: false);
}