// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Autofac;

/// <summary>
/// Disposable implementation of <see cref="IServiceScope"/>.
/// </summary>
internal sealed class ServiceScope : IServiceScope
{
    private bool isDisposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceScope"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    internal ServiceScope(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        (this.ServiceProvider as IDisposable)?.Dispose();
        this.isDisposed = true;
    }

    /// <summary>
    /// Gets the scoped service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }
}