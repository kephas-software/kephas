// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionScopedCompositionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi scoped composition context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A scoped composition context for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class DependencyInjectionScopedCompositionContext : DependencyInjectionCompositionContextBase
    {
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionScopedCompositionContext"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        public DependencyInjectionScopedCompositionContext(IServiceScope serviceScope)
            : base(serviceScope.ServiceProvider)
        {
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public override void Dispose()
        {
            this.serviceScope.Dispose();
        }
    }
}