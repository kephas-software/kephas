// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using Kephas.Injection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// An injection service scope factory.
    /// </summary>
    public class InjectionServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public InjectionServiceScopeFactory(IInjector injector)
        {
            this.injector = injector;
        }

        /// <summary>
        /// Create an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> which
        /// contains an <see cref="T:System.IServiceProvider" /> used to resolve dependencies from a
        /// newly created scope.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> controlling the
        /// lifetime of the scope. Once this is disposed, any scoped services that have been resolved
        /// from the <see cref="P:Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider" />
        /// will also be disposed.
        /// </returns>
        public IServiceScope CreateScope()
        {
            return new InjectionServiceScope(this.injector);
        }
    }
}