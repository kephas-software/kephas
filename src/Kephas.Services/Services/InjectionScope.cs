// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// The injection scope.
/// </summary>
public class InjectionScope : IInjectionScope
{
    private readonly bool ownsServiceProvider;
    private bool isDisposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectionScope"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for this scope.</param>
    /// <param name="ownsServiceProvider">Indicates whether disposing the scope will dispose the service provider.</param>
    public InjectionScope(IServiceProvider serviceProvider, bool ownsServiceProvider = true)
    {
        this.ownsServiceProvider = ownsServiceProvider;
        this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets the service provider for this scope.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from <see cref="Dispose()"/> (<c>true</c>) or finalizer ( (<c>false</c>)).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (this.ownsServiceProvider && this.ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}