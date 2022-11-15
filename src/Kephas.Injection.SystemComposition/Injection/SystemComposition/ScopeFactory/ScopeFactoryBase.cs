// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeFactoryBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.ScopeFactory
{
    using System.Composition;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    public abstract class ScopeFactoryBase : IScopeFactory
    {
        /// <summary>
        /// The scoped context factory.
        /// </summary>
        private readonly ExportFactory<CompositionContext> scopedContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeFactoryBase"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        protected ScopeFactoryBase(ExportFactory<CompositionContext> scopedContextFactory)
        {
            scopedContextFactory = scopedContextFactory ?? throw new System.ArgumentNullException(nameof(scopedContextFactory));

            this.scopedContextFactory = scopedContextFactory;
        }

        /// <summary>
        /// Creates the scoped context export.
        /// </summary>
        /// <returns>
        /// The new scoped context export.
        /// </returns>
        public virtual Export<CompositionContext> CreateScopedContextExport()
        {
            return this.scopedContextFactory.CreateExport();
        }
    }
}
