// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScopedCompositionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac scoped composition context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using global::Autofac;

    /// <summary>
    /// An Autofac scoped composition context.
    /// </summary>
    public class AutofacScopedCompositionContext : AutofacCompositionContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacScopedCompositionContext"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public AutofacScopedCompositionContext(ILifetimeScope scope)
        {
            this.Initialize(scope);
        }
    }
}