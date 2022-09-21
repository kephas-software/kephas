// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceScope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service scope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;

    using Kephas.Injection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service scope.
    /// </summary>
    public sealed class InjectionServiceScope : IServiceScope
    {
        private readonly IInjectionScope scope;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionServiceScope"/> class.
        /// </summary>
        /// <param name="parentInjector">Context for the parent composition.</param>
        public InjectionServiceScope(IInjector parentInjector)
        {
            this.scope = parentInjector.CreateScope();
            this.ServiceProvider = this.scope.Injector.ToServiceProvider();
        }

        /// <summary>
        /// Gets the <see cref="T:System.IServiceProvider" /> used to resolve dependencies from the scope.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.scope.Dispose();
            }
        }
    }
}