// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionServiceScope.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition service scope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Composition
{
    using System;

    using Kephas.Composition;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition service scope.
    /// </summary>
    public class CompositionServiceScope : IServiceScope
    {
        /// <summary>
        /// Context for the scoped composition.
        /// </summary>
        private readonly ICompositionContext scopedCompositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceScope"/> class.
        /// </summary>
        /// <param name="parentCompositionContext">Context for the parent composition.</param>
        public CompositionServiceScope(ICompositionContext parentCompositionContext)
        {
            this.scopedCompositionContext = parentCompositionContext.CreateScopedContext();
            this.ServiceProvider = this.scopedCompositionContext.ToServiceProvider();
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
            this.scopedCompositionContext.Dispose();
        }
    }
}