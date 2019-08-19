// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediScopedCompositionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi scoped composition context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Hosting
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A scoped composition context for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class MediScopedCompositionContext : MediCompositionContextBase
    {
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediScopedCompositionContext"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope.</param>
        /// <param name="scopeName">The name of the scope.</param>
        public MediScopedCompositionContext(IServiceScope serviceScope, string scopeName)
            : base(serviceScope.ServiceProvider)
        {
            this.ScopeName = scopeName;
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>
        /// The name of the scope.
        /// </value>
        public string ScopeName { get; }

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