// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectionScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac;

/// <summary>
/// Injection scope for Autofac.
/// </summary>
internal class AutofacInjectionScope : IInjectionScope
{
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutofacInjectionScope"/> class.
    /// </summary>
    /// <param name="injector">The injector.</param>
    public AutofacInjectionScope(IInjector injector)
    {
        this.Injector = injector ?? throw new ArgumentNullException(nameof(injector));
    }

    /// <summary>
    /// Gets the injector for this scope.
    /// </summary>
    public IInjector Injector { get; }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.Injector.Dispose();
        this.disposed = true;
    }
}