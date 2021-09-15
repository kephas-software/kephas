// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionServiceScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service scope factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service scope factory.
    /// </summary>
    public class CompositionServiceScopeFactory : IServiceScopeFactory
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="injector">Context for the composition.</param>
        public CompositionServiceScopeFactory(IInjector injector)
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
            return new CompositionServiceScope(this.injector);
        }
    }
}